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

}
