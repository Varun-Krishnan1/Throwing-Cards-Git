using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : ObjectTrigger
{
    public Animator thisLightAnimator; 

    public override void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "ThrownPlayerCard" || collider.gameObject.tag == "ThrownCane")
        {
            // -- destroy light here 
            if(thisLightAnimator != null)
            {
                thisLightAnimator.SetBool("hasBeenHit", true);
            }

            // -- activate trigger 
            triggerActivated = true;

        }
    }

}
