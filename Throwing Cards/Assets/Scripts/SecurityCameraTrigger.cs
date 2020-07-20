using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraTrigger : ObjectTrigger 
{
    public Rigidbody2D cameraRB; 
    public SecurityCameraRotation cameraRot; 

    public override void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "ThrownPlayerCard" || collider.gameObject.tag == "ThrownCane")
        {
            print("HERe");
            // -- stop rotation 
            Destroy(cameraRot);

            // -- enable falling movement 
            cameraRB.bodyType = RigidbodyType2D.Dynamic; // -- change from kinematic -> dynamic so affected by collisions 
            cameraRB.constraints = RigidbodyConstraints2D.None; // -- remove freeze position constraints 

            // -- activate trigger 
            triggerActivated = true;
        }
    }
}
