using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -- BASE CLASS ALL WEAPONS INHERIT FROM 
public abstract class ObjectTrigger : MonoBehaviour
{

   protected bool triggerActivated = false; 

   public abstract void OnCollisionEnter2D(Collision2D collider);

   public bool isTriggered()
    {
        return triggerActivated;
    }
}