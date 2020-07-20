using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : ObjectTrigger
{
    public override void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "ThrownPlayerCard" || collider.gameObject.tag == "ThrownCane")
        {
            // -- destroy light here 

            // -- activate trigger 
            triggerActivated = true;
        }
    }

}
