using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraTrigger : ObjectTrigger 
{
    public Rigidbody2D cameraRB; 
    public SecurityCameraRotation cameraRot; 
    public float fallKillSpeed; 

    public override void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "ThrownPlayerCard" || collider.gameObject.tag == "ThrownCane")
        {
            // -- stop rotation 
            Destroy(cameraRot);

            // -- enable falling movement 
            cameraRB.bodyType = RigidbodyType2D.Dynamic; // -- change from kinematic -> dynamic so affected by collisions 
            cameraRB.constraints = RigidbodyConstraints2D.None; // -- remove freeze position constraints 

            // -- activate trigger 
            triggerActivated = true;
        }
        // -- if it falls on player kill them lol 
        else if (collider.gameObject.tag == "Player" && collider.relativeVelocity.y > fallKillSpeed)
        {
            collider.gameObject.GetComponent<PlayerHealthController>().TakeDamage(100); 
        }
        print(collider.relativeVelocity.y);

    }
}
