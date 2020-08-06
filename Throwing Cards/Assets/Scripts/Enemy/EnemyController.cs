using System; 
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health;  
    public float fHorizontalDampingWhenMoving;
    public float knockbackForce;
    public HealthBar healthBar;
    public bool stationary = false; 

    [Header("Attacking")]
    public LayerMask playerMask;
    public float playerCheckDistance;
    public Transform playerCheckPosition;
    public float moveSpeedWhileCharging;
    public float chargingTime;
    public Collider2D headCollider;
    public Collider2D targetCollider; 

    private bool charging;
    private float currentChargingTime; 

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed;
    public Transform groundDetection;
    public float checkDistance;
    public bool movingRight = false;
    public LayerMask wallMask;
    public LayerMask groundMask;
    private Vector2 dir; 



    [Header("Death Effect")]
    public GameObject deathEffect;
    public float offsetX;
    public float offsetY; 


    private GameObject player;


    private Animator animator;

    /* State Machine */
    private State state;
    private enum State
    {
        Normal,
        Charging,
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        healthBar.SetMaxHealth(health);

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(!stationary)
        {
            dir = Vector2.left;
            if (movingRight)
            {
                dir = Vector2.right;
            }
            // -- uses variable state that was obtained in UPDATE 
            switch (state)
            {
                case State.Normal:
                    headCollider.enabled = true;
                    targetCollider.enabled = true;

                    animator.SetBool("isCharging", false);

                    TurnLogic();

                    /* Stop and attack if player right in front */
                    RaycastHit2D playerInfo = Physics2D.Raycast(playerCheckPosition.position, dir, playerCheckDistance, playerMask);
                    if (playerInfo.collider != null)
                    {
                        print("Attacking Player!");
                        rb.velocity = Vector3.zero;
                        animator.SetBool("isAttacking", true);
                    }
                    else
                    {
                        Move(moveSpeed);
                        animator.SetBool("isAttacking", false);
                    }

                    /* Charge if player in line of sight */
                    RaycastHit2D playerChargeInfo = Physics2D.Raycast(playerCheckPosition.position, dir, Mathf.Infinity, playerMask);
                    if (playerChargeInfo.collider != null && !charging)
                    {
                        state = State.Charging;
                        currentChargingTime = chargingTime;
                    }

                    break;
                case State.Charging:
                    /* Disable head and circle collider */
                    headCollider.enabled = false;
                    targetCollider.enabled = false;


                    rb.velocity = Vector3.zero;
                    animator.SetBool("isCharging", true);

                    RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, checkDistance, groundMask);
                    RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, dir, checkDistance, wallMask);


                    if (currentChargingTime < 0)
                    {
                        state = State.Normal;
                    }
                    else if (groundInfo.collider == null || wallInfo.collider == true)
                    {
                        rb.velocity = Vector3.zero;
                        state = State.Normal;
                    }
                    else
                    {
                        Move(moveSpeedWhileCharging);
                    }

                    currentChargingTime -= Time.fixedDeltaTime;
                    break;
            }
        }







    }

    /* Returns true if enemy turns that frame */ 
    private bool TurnLogic()
    {
        /* Rotate if hit wall or end of platform */
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, checkDistance, groundMask);
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, dir, checkDistance, wallMask);
        if (groundInfo.collider == false || wallInfo.collider == true)
        {
            print("Rotate called");
            transform.Rotate(0, 180f, 0);
            movingRight = !movingRight;
            rb.velocity = Vector3.zero;

            // get direction again 
            dir = Vector2.left;
            if (movingRight)
            {
                dir = Vector2.right;
            }

            return true; 
        }

        return false; 
    }

    void Move(float moveSpeed)
    {
        Vector2 dir = Vector2.left;

        if (movingRight)
        {
            dir = Vector2.right; 
        }

        rb.velocity = dir * moveSpeed; 
    }
    


    public void TakeDamage(int damage, GameObject weapon)
    {
        // -- knockback 
        // -- used so not too much knockback 

        Vector2 direction = this.transform.position - weapon.transform.position;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);



        // -- take the actual damage 
        health -= damage;
        print("DAMAGE TAKEN: " + damage);
        healthBar.SetHealth(health);
        if(health <= 0)
        {
            Die(); 
        }


    }



    public void Die()
    {
        print(this.gameObject.transform.position);
        GameObject e = Instantiate(deathEffect, new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0), Quaternion.Euler(0,180,0));
        print(e.transform.position);
        Destroy(gameObject);
    }

    // -- attacking player 
    void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player" && state == State.Charging)
        {
            hitInfo.gameObject.GetComponent<PlayerHealthController>().TakeDamage(100); 
        }
        // -- if moving and hits card break card 
        if(hitInfo.gameObject.tag == "ThrownPlayerCard")
        {
            hitInfo.gameObject.GetComponent<CardController>().UnFreeze(); 
        }
    }
}
