using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health;  
    public float moveSpeedWhenHit;
    public float fHorizontalDampingWhenMoving;
    public float knockbackForce;
    public HealthBar healthBar;


    private GameObject player;

    private float moveSpeed;
    private bool moveRight = false; 
    private Rigidbody2D rb;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        healthBar.SetMaxHealth(health);
        player = GameObject.FindWithTag("Player");

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float fHorizontalVelocity = rb.velocity.x;

        // -- if enemy gets moved cause it to run and it hasn't been hit yet (second condition) 
        if(fHorizontalVelocity > 0)
        {
            moveSpeed = moveSpeedWhenHit; 
        }

        if (!moveRight)
        {
            fHorizontalVelocity -= moveSpeed;

        }
        else
        {
            fHorizontalVelocity += moveSpeed;
        }

        fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingWhenMoving, Time.fixedDeltaTime * 10f);



        rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);

        animator.SetFloat("speed", Math.Abs(fHorizontalVelocity));


    }

    public void TakeDamage(int damage)
    {
        // -- knockback 
        // -- used so not too much knockback 
        rb.AddForce(new Vector2(knockbackForce, knockbackForce), ForceMode2D.Impulse); 


        // -- take the actual damage 
        health -= damage;
        print("DAMAGE TAKEN: " + damage);
        healthBar.SetHealth(health);
        if(health <= 0)
        {
            Die(); 
        }

        // -- get direction player is facing 
        bool player_facing_right = player.transform.rotation.y != -1;
        bool this_facing_right = this.transform.rotation.y != 1;        // have to compare it to 1 because it's starts facing left... 
        // -- start running when they get hit
        moveSpeed = moveSpeedWhenHit;

        print(player_facing_right);
        print("E:" + this_facing_right);
        // -- rotate if enemy not flacing player (i.e rotate if they're pointing in same direction) 
        if (player_facing_right == this_facing_right)
        {
            print("Rotating to face player...");    
            // -- rotate 
            transform.Rotate(0, 180f, 0);
            moveRight = true; 
        }

    }



    public void Die()
    {
        Destroy(gameObject);
    }

    // -- attacking player 
    void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player")
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
