using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    /* Insepctor Variables */ 
    public Controller2D controller;
    public Animator animator;
    public float runSpeed = 40f;

    /* Internal Variables */ 
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool jumpHeight = false; 

    private float fJumpPressedRememberTime = 0.2f;      // amount of time script remembers user pressed jump (allows for jump buffering) 
    float fJumpPressedRemember = 0f;
    float jumpTimer = 0f;
    float jumpTimerTime = .5f; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        jumpTimerTime -= Time.deltaTime; 
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true; 
            fJumpPressedRemember = fJumpPressedRememberTime;
            jumpTimer = jumpTimerTime; 

        }

        if(Input.GetButtonUp("Jump"))
        {
            jumpHeight = true; 
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
            crouch = false;
    }

    public void BufferedJump()
    {
        if (fJumpPressedRemember > 0)
        {
            print("Buffered Jump!");
            jump = true;
            fJumpPressedRemember = 0;
        }
    }



    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);

        // -- cancel shooting animation 
        animator.SetBool("isShooting", false);
    }

    // Fixed Update is called a fixed amount of time per second (better for physics)

    void FixedUpdate()
    {
        fJumpPressedRemember -= Time.fixedDeltaTime;
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, jumpHeight);

        // -- Move Animations 
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if(Mathf.Abs(horizontalMove) > .01)
        {
            // -- cancel shooting animation and cane loading animation 
            animator.SetBool("isShooting", false);
            animator.SetBool("isLoadingCane", false); 
        }

        jump = false;
        jumpHeight = false; 

    }
}
