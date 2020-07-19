using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Controller2D : MonoBehaviour
{
	[SerializeField] private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
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
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;

	/* Better Jumping */
	public Animator animator;

	private float bufferedJumpWindowTime = .2f;     // When onLanding gets invoked buffered window starts at this time 
	private float bufferedJumpWindowAllowed = .1f;  // Only when buffered jump window hits THIS time is buffered jumping allowed 
													// needed because onLanding gets invoked very early due to grounded radius 
													// nut don't want them to be able to buffer jump that early so wait till it's this variable 
	private float bufferedJumpWindow = 0f;
	private float jumpTimer = 0;				
	private float jumpTimerTime = .2f;              // Amount of time before they are able to jump again after jumping (prevents spamming of jump) 

	private float groundedTimer = 0f;
	private float groundedTimerTime = .05f;         // Allows for coyote jumping off platforms because it remembers when you were grounded for a small amount of time 
													// so even if you miss jump and fall off it still remembers you were grounded! 

	private float groundAnimation = 0f;
	private float groundAnimationTime = .05f;

	private float ceilingTimer = 0f;
	private float ceilingTimerTime = .05f; 

	public float fCutJumpHeight = .2f; 
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent BufferedJumpEvent; 

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		jumpTimer -= Time.fixedDeltaTime; 
		bufferedJumpWindow -= Time.fixedDeltaTime;
		groundedTimer -= Time.fixedDeltaTime;
		groundAnimation -= Time.fixedDeltaTime;
		ceilingTimer -= Time.fixedDeltaTime; 

		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (groundAnimation < 0) 
				{
					OnLandEvent.Invoke();
					bufferedJumpWindow = bufferedJumpWindowTime;

				}

				// -- anytime you detect ground reset timer for ground animation buffering (i.e how long it remembers you're on the ground for) 
				groundAnimation = groundAnimationTime;

				// -- whenever you find ground reset timer for cayote jumping 
				groundedTimer = groundedTimerTime;

			}
		}


		if (bufferedJumpWindow > 0 && bufferedJumpWindow <= bufferedJumpWindowAllowed)
        {
			BufferedJumpEvent.Invoke();
        }

		// -- if you hit ceiling recently while you are also grounded stop the jumping animation 
		if (wasGrounded && ceilingTimer > 0)
        {
			animator.SetBool("isJumping", false);
		}

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

				// -- keep track if they hit the ceiling recently 
				ceilingTimer = ceilingTimerTime; 
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
					OnCrouchEvent.Invoke(true);
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
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			//Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			//// And then smoothing it out and applying it to the character
			//m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			float fHorizontalVelocity = m_Rigidbody2D.velocity.x;
			fHorizontalVelocity += move;

			if (Mathf.Abs(move) < 0.01f)
				fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingWhenStopping, Time.fixedDeltaTime * 10f);
			else if (Mathf.Sign(move) != Mathf.Sign(fHorizontalVelocity))
				fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingWhenTurning, Time.fixedDeltaTime * 10f);
			else
				fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

			m_Rigidbody2D.velocity = new Vector2(fHorizontalVelocity, m_Rigidbody2D.velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (groundedTimer >= 0 && jump && jumpTimer <= 0)
		{ 
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

			// -- put grounded timer to zero and start jump timer 
			jumpTimer = jumpTimerTime;
			groundedTimer = 0;
			m_Grounded = false;


			// -- particle effect 
			CreateDust();

			// -- jump animation 
			animator.SetBool("isJumping", true);
			
			// -- cancel shooting animation and cane loading animation 
			animator.SetBool("isShooting", false);
			animator.SetBool("isLoadingCane", false);



		}


		// changing jump height based on how long they pressed jump for
		if (jumpHeight)
        {
			if (m_Rigidbody2D.velocity.y > 0)
			{
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,
					m_Rigidbody2D.velocity.y * fCutJumpHeight);
			}
		}
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