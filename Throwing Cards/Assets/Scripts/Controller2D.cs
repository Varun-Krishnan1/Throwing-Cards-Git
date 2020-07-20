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
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private ParticleSystem m_Dust; 

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_wasCrouching = false; // Whether or not the player was crouching 
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up

	/* Rigidbody Stuff */ 
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;



	/* Better Jumping */
	public Animator animator;

	private float jumpTimer = 0;				
	private float jumpTimerTime = .2f;              // Amount of time before they are able to jump again after jumping (prevents spamming of jump) 

	private float groundedTimer = 0f;
	private float groundedTimerTime = .05f;         // Allows for coyote jumping off platforms because it remembers when you were grounded for a small amount of time 
													// so even if you miss jump and fall off it still remembers you were grounded! 

	private float fJumpPressedRememberTime = 0.2f;      // amount of time script remembers user pressed jump (allows for jump buffering) 
	private float fJumpPressedRemember = 0f;


	/* Internal Input Varibles  */
	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;
	bool jumpHeight = false;



	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

	}

	// Update is called once per frame
	private void Update()
	{
		// -- decrementing an input jump timer goes here 
		jumpTimer -= Time.deltaTime;

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

		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		}
		else if (Input.GetButtonUp("Crouch"))
			crouch = false;
	}

	private void FixedUpdate()
	{
		// -- decrement global timers 
		groundedTimer -= Time.fixedDeltaTime;  // cayote timer 
		fJumpPressedRemember -= Time.fixedDeltaTime; // buffered jump timer 

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

		// -- this is key for not needing an onlanding event! 
		if(m_Grounded)
        {
			animator.SetBool("isJumping", false);

			// -- whenever you find ground reset timer for cayote jumping 
			groundedTimer = groundedTimerTime;
		}
		else
        {
			animator.SetBool("isJumping", true);
		}

		// -- jump if you pressed jump recently(jump buffering) or were grounded recently (cayote jumping) 
		// -- jumpTimer variable is to prevent spam jumping 
		if (fJumpPressedRemember > 0 && groundedTimer >= 0 && jumpTimer <= 0)
		{
			print("Jump Here!");
			animator.SetBool("isJumping", true);

			jump = true;

			// -- reset jump buffering 
			fJumpPressedRemember = 0;

			// -- reset cayote timer 
			groundedTimer = 0;

			// -- reset jump timer to prevent spamming 
			jumpTimer = jumpTimerTime;

		}
		
		// -- actual moving AND animations occurs in this function (besides jump animations) 
		this.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, jumpHeight);

		// -- reset variables 
		jump = false;
		jumpHeight = false;

	}


	public void Move(float move, bool crouch, bool jump, bool jumpHeight)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded|| m_AirControl)
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

			if (move > .01)
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

		// If player should jump 
		if (jump)
		{
			print("JUMP THERE");
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

			// -- particle effect 
			CreateDust();
			
			// -- cancel shooting animation and cane loading animation 
			animator.SetBool("isShooting", false);
			animator.SetBool("isLoadingCane", false);

		}

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