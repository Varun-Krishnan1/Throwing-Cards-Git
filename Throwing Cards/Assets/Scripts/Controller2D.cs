using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Controller2D : MonoBehaviour
{
	[SerializeField] private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	[SerializeField] public float runSpeed = 40f;
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[SerializeField] public float fCutJumpHeight = .5f;							// Amount to cut jump height by when releasing jump button 
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = 0f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, 1)] [SerializeField] private float fHorizontalDampingBasic = .22f;     // How much to smooth out the movement
	[Range(0, 1)] [SerializeField] private float fHorizontalDampingWhenStopping = .22f;
	[Range(0, 1)] [SerializeField] private float fHorizontalDampingWhenTurning = .22f;
	[Range(0, 1)] [SerializeField] private float fHorizontalDampingWhenRolling = .22f; 
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;					   // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private Collider2D m_BodyCollider;							// Body capsule collider of player 
	[SerializeField] private PhysicsMaterial2D m_OnCardMaterial;				// Material to change to while on card 
	[SerializeField] private PhysicsMaterial2D m_OnGroundMaterial;				// Material to change to while on ground 
	[SerializeField] private ParticleSystem m_Dust; 

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_wasCrouching = false; // Whether or not the player was crouching 
	const float k_CeilingRadius = 1f; // Radius of the overlap circle to determine if the player can stand up
	private bool unPressedCrouch = false;
	private float dynamicRadiusLength = .4f; 

	/* Rigidbody Stuff */
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;

	/* Internal Input Varibles  */
	float horizontalMove = 0f;
	bool crouch = false;
	bool jumpHeight = false;

	/* Better Jumping */
	public Animator animator;

	private float jumpTimer = 0;				
	private float jumpTimerTime = .2f;              // Amount of time before they are able to jump again after jumping (prevents spamming of jump) 

	private float groundedTimer = 0f;
	private float groundedTimerTime = .05f;         // Allows for coyote jumping off platforms because it remembers when you were grounded for a small amount of time 
													// so even if you miss jump and fall off it still remembers you were grounded! 

	private float fJumpPressedRememberTime = 0.2f;      // amount of time script remembers user pressed jump (allows for jump buffering) 
	private float fJumpPressedRemember = 0f;

	/* Wall Sliding and Jumping */
	[SerializeField] private LayerMask m_WhatIsWall;
	public Transform WallCheck;
	public float wallSlidingSpeed;
	public float xWallForce;
	public float yWallForce;
	const float k_WallRadius = .2f;

	private bool m_TouchingWall;
	private bool isWallSliding;

	private bool wallJumping;
	public float wallJumpTime;

	/* Rolling */
	public float rollingSpeed;
	public float totalRollTime;
	private float rollTime;
	private float rollCooldownTimer = 0f; 


	/* Testing */
	private CardController prevCard;

	/* State Machine */ 
	private State state; 
	private enum State
    {
		Normal, 
		Rolling,
    }

	void changeMaterial(PhysicsMaterial2D mat)
    {
		m_BodyCollider.sharedMaterial = mat;

	}

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

	}

	// Update is called once per frame
	private void Update()
	{
		// -- decrementing an input jump timer goes here 
		jumpTimer -= Time.deltaTime;
		rollCooldownTimer -= Time.deltaTime; 

		// -- get inputs 
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		if (Input.GetButtonDown("Jump"))
		{
			fJumpPressedRemember = fJumpPressedRememberTime;
		}

		// -- no jump height functionality for now 
		/*if (Input.GetButtonUp("Jump"))
		{
			jumpHeight = true;
		}*/

		// -- if they press crouch and not jumpign (this can be done better with a state machine) 
		if (Input.GetButtonDown("Crouch") && m_Rigidbody2D.velocity.y == 0)
		{
			// -- if moving and not in air or already crouching then it's a roll 
			if (Mathf.Abs(horizontalMove) > .01 && rollCooldownTimer <= 0 && !crouch)
			{
				// -- tell fixed update roll was pressed 
				state = State.Rolling;
				// -- start roll timer 
				rollTime = totalRollTime;

				// -- set roll cooldown timer for how long it takes to do a roll
				rollCooldownTimer = rollTime; 
			}
			else
            {
				crouch = true;
			}
		}
		else if (Input.GetButtonUp("Crouch"))
        {
			crouch = false;
			unPressedCrouch = true;
		}
	}

	private void FixedUpdate()
	{
		// -- decrement global timers 
		groundedTimer -= Time.fixedDeltaTime;  // cayote timer 
		fJumpPressedRemember -= Time.fixedDeltaTime; // buffered jump timer 

		setGlobalGrounded();

		dynamicallyChangeMaterial(dynamicRadiusLength);

		// -- uses variable state that was obtained in UPDATE 
		switch (state)
        {
			case State.Normal:
				// -- uses variables fJumpPressedRemember, groundedTimer, and jumpTimer that was obtained in UPDATE 
				JumpingLogic();

				// -- uses variables m_Grounded that was obtained in FIXED-UPDATE and horizontalMove which was obtained in UPDATE 
				WallSlidingLogic();

				// -- uses variable horizontalMove which was obtained in UPDATE 
				MoveAndCrouch();

				// -- uses variable horizontalMove which was obtained in UPDATE 
				FlipLogic();
				break; 
			case State.Rolling:
				// -- uses variables m_Grounded in FIXED UPDATE and horizontalMove
				RollingLogic();
				break; 
		}



	}

	private void RollingLogic()
    {
		float fHorizontalVelocity = m_Rigidbody2D.velocity.x;
		animator.SetBool("isRolling", true);

		if (this.isFacingRight())
        {
			fHorizontalVelocity += rollingSpeed;
		}
		else
        {
			fHorizontalVelocity -= rollingSpeed;
		}

		fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingWhenRolling, Time.fixedDeltaTime * 10f);

		m_Rigidbody2D.velocity = new Vector2(fHorizontalVelocity, m_Rigidbody2D.velocity.y);

		// Disable one of the colliders when rolling
		if (m_CrouchDisableCollider != null)
			m_CrouchDisableCollider.enabled = false;

		// -- stop after certain time 
		if (rollTime < 0)
        {
			state = State.Normal;
			animator.SetBool("isRolling", false);

			// -- if ceiling above when they're done rolling put them in crouch 
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}

		}

		rollTime -= Time.fixedDeltaTime;

	}

	private void WallSlidingLogic()
	{
		m_TouchingWall = Physics2D.OverlapCircle(WallCheck.position, k_WallRadius, m_WhatIsWall);
		// -- if touching wall and moving into wall with arrow key and not grounded
		if (m_TouchingWall && !m_Grounded && Mathf.Abs(horizontalMove) > 0)
		{
			isWallSliding = true;
		}
		else
		{
			isWallSliding = false;
		}

		if(isWallSliding)
        {
			animator.SetBool("isWallSliding", true);
			animator.SetBool("isJumping", false);

			// -- particle effect 
			CreateDust();

			// -- essentialy change y velocity to b/w wall-sliding speed value(which is NEGATIVE) and max value 
			m_Rigidbody2D.velocity = 
				new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, -wallSlidingSpeed, float.MaxValue)); 
        }
		else
        {
			animator.SetBool("isWallSliding", false);
		}

		// -- if they pressed jump and they are wall sliding 
		if(fJumpPressedRemember > 0 && isWallSliding)
        {

			wallJumping = true;
			// -- after certain amount of time stop wall jumping 
			Invoke("SetWallJumpingToFalse", wallJumpTime);

			fJumpPressedRemember = 0f; 
		}

		if(wallJumping)
        {
			animator.SetBool("isWallSliding", false);
			animator.SetBool("isJumping", true);

			// -- particle effect 
			CreateDust();

			m_Rigidbody2D.velocity = new Vector2(xWallForce * -horizontalMove, yWallForce); 
        }

	}

	void SetWallJumpingToFalse()
    {
		wallJumping = false; 
    }

	public void setGlobalGrounded()
    {
		// -- check for grounded 
		m_Grounded = false;
		
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
			}
		}
	}

	private void dynamicallyChangeMaterial(float radiusCheckLength)
    {
		// -- NOTE:  the commented sections have the potential to have the functionality every time he moves off a card it gets destroyed 

		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, radiusCheckLength, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				CardController card = colliders[i].gameObject.GetComponent<CardController>();
				if (card != null)
				{
					//card.changeMaterial(m_OnCardMaterial);
					this.changeMaterial(m_OnCardMaterial);

					/* 
					if(prevCard != null && prevCard != card)
					{
						Destroy(prevCard.gameObject);
					}
					// -- if they jump off keep track of last card they were on 
					prevCard = card;
					*/

					// -- the minute you find a card nearby change material and end function 
					return; 
				}
				else
				{
					/* 
					if (prevCard != null)
					{
						print("Jumped off card"); 
						//prevCard.changeMaterial(m_OnGroundMaterial);
						//prevCard.toggleFreezeState(); 
						Destroy(prevCard.gameObject);
					}
					*/
					this.changeMaterial(m_OnGroundMaterial);
				}

			}
		}
		
	}

	public void JumpingLogic()
    {
		// -- this is key for not needing an onlanding event! 
		if (m_Grounded)
		{
			animator.SetBool("isJumping", false);


			// -- whenever you find ground reset timer for cayote jumping 
			groundedTimer = groundedTimerTime;
		}
		// -- this is for like falling without pressing jump key 
		else
		{
			animator.SetBool("isJumping", true);
		}

		// uses variables that were obtained in update to jump 

		// -- jump if you pressed jump recently(jump buffering) or were grounded recently (cayote jumping) 
		// -- jumpTimer variable is to prevent spam jumping 
		if (fJumpPressedRemember > 0 && groundedTimer >= 0 && jumpTimer <= 0)
		{
			// -- if falling then it's a buffered jump and then wait till velocity is zero 
			if (m_Rigidbody2D.velocity.y < 0)
			{
				// -- don't jump or reset just wait till they hit ground and velocity is zero  
			}
			else
			{
				animator.SetBool("isJumping", true);
				animator.SetBool("isWallSliding", false);


				Jump();

				// -- reset jump buffering 
				fJumpPressedRemember = 0;

				// -- reset cayote timer 
				groundedTimer = 0;

				// -- reset jump timer to prevent spamming 
				jumpTimer = jumpTimerTime;
			}
		}
	}
	// -- move and crouch are in same function because crouch affects move speed
	public void MoveAndCrouch()
	{
		float move = horizontalMove * Time.fixedDeltaTime;

		// If they are now not crouching, keep player crouching if ceiling above them 
		if (unPressedCrouch && !isWallSliding && m_Rigidbody2D.velocity.y == 0)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
				unPressedCrouch = true; 
			}
			// - if they have unpressed crouch and you didn't find a ceiling set crouch to false  
			else 
            {
				crouch = false;
				unPressedCrouch = false; 
            }
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					animator.SetBool("isCrouching", true);

					// -- cancel shooting animation 
					animator.SetBool("isShooting", false);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					animator.SetBool("isCrouching", false);

				}
			}

			// -- Move Animations 
			animator.SetFloat("Speed", Mathf.Abs(move));

			if (Mathf.Abs(move) > .01)
			{
				// -- cancel shooting animation and cane loading animation 
				animator.SetBool("isShooting", false);
				animator.SetBool("isLoadingCane", false);
			}

			// -- Moving rigidbody 
			float fHorizontalVelocity = m_Rigidbody2D.velocity.x;
			fHorizontalVelocity += move;

			if (Mathf.Abs(move) < 0.01f)
				fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingWhenStopping, Time.fixedDeltaTime * 10f);
			else if (Mathf.Sign(move) != Mathf.Sign(fHorizontalVelocity))
				fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingWhenTurning, Time.fixedDeltaTime * 10f);
			else
				fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

			m_Rigidbody2D.velocity = new Vector2(fHorizontalVelocity, m_Rigidbody2D.velocity.y);
		}
	}

	public void FlipLogic()
    {
		float move = horizontalMove * Time.fixedDeltaTime;

		// -- flipping player 
		// If the input is moving the player right and the player is facing left...
		if (move > 0 && !m_FacingRight)
		{
			Flip();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (move < 0 && m_FacingRight)
		{
			Flip();
		}
	}

	public void Jump()
    {

		// Add a vertical force to the player

		// -- for jump buffering first got to ensure velocity is zero 
		m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

		// -- particle effect 
		CreateDust();

		// -- cancel shooting animation and cane loading animation 
		animator.SetBool("isShooting", false);
		animator.SetBool("isLoadingCane", false);

		// -- no jump height functionality for now 
		// changing jump height based on how long they pressed jump for
		// -- for buffered jumps(when the button is pressed essentially when they're grounded) don't run this function 
		/*if (jumpHeight)
        {
			if (m_Rigidbody2D.velocity.y > 0)
			{
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,
					m_Rigidbody2D.velocity.y * fCutJumpHeight);
			}
		}*/
	}


	public void CreateDust()
    {
		m_Dust.Play(); 
    }

	// -- actual physical flipping 
    public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0, 180f, 0);

		// dust effect when flipping but not when in air 
		if (m_Grounded)
		{
			CreateDust();
		}
	}

	public bool isFacingRight()
    {
		return m_FacingRight; 
    }
}