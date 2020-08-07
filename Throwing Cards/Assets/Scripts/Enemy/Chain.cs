using UnityEngine;

public class Chain : MonoBehaviour
{
    public float chainForce;
    public float minHitSpeed;
    public bool facingRight = false; 
    public Rigidbody2D rb; 

    // Start is called before the first frame update
    void Awake()
    {
        if(facingRight)
        {
            chainForce = -chainForce; 
        }
    }

    void FixedUpdate()
    {
        // -- force is left 
        float force = -chainForce * Time.fixedDeltaTime; 
        rb.AddForce(new Vector2(force, force)); 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && (rb.velocity.x > minHitSpeed || rb.velocity.y > minHitSpeed))
        {
            collision.gameObject.GetComponent<PlayerHealthController>().TakeDamage(100);
        }
    }
}
