using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : ObjectTrigger
{
    public Animator thisLightAnimator;
    public HingeJoint2D hinge; 
    public float delay = 0f;

    public override void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "ThrownPlayerCard" || collider.gameObject.tag == "ThrownCane")
        {
            // -- destroy light here if has animator component 
            if(thisLightAnimator != null)
            {
                Invoke("setHitTrue", delay); 
            }
            if(hinge != null)
            {
                Destroy(hinge); 
            }

            // -- activate trigger 
            triggerActivated = true;

        }

    }

    void setHitTrue()
    {
        thisLightAnimator.SetBool("hasBeenHit", true);
    }
}
