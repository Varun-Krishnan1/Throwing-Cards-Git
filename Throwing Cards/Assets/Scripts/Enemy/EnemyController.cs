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

    [Header("Movement")]
    public float moveSpeed;
    [Range(0, 1)] [SerializeField] private float fHorizontalDampingBasic = .22f;     // How much to smooth out the movement
    public Transform groundDetection;
    public float checkDistance;
    public bool movingRight = false;

    private Rigidbody2D rb;


    [Header("Death Effect")]
    public GameObject deathEffect;
    public float offsetX;
    public float offsetY; 


    private GameObject player;


    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        //animator = this.GetComponent<Animator>();
        healthBar.SetMaxHealth(health);

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, checkDistance);
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, checkDistance);
        print(groundInfo.transform); 
        if(groundInfo.collider == false)
        {
            print("Edge of surface");  
            transform.Rotate(0, 180f, 0);
            movingRight = !movingRight;
        }

        Move();
    }

    void Move()
    {
        float fHorizontalVelocity = rb.velocity.x;

        
        if (movingRight)
        {
            fHorizontalVelocity += .5f;
        }
        else
        {
            fHorizontalVelocity -= .5f;
        }

        fHorizontalVelocity *= (float)Math.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
        rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);
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
