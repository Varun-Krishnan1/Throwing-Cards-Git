using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkToCanePickup : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && !triggered)
        {
            AudioManager.instance.Play("PickupItem"); 
            triggered = true;

            // -- add weapon to weapons array
            col.gameObject.GetComponent<WeaponsManager>().AddWeapon("SilkToCane"); 

            // -- display notification 
            Notifications.instance.AddNotification("New Magic Trick Found: Silk to Cane");

            // -- destroy self after collected 
            Destroy(gameObject);
        }
    }
}
