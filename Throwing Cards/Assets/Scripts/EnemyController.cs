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
        //animator = this.GetComponent<Animator>();
        healthBar.SetMaxHealth(health);
        player = GameObject.FindWithTag("Player");

    }


    // Update is called once per frame
    void FixedUpdate()
    {


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
