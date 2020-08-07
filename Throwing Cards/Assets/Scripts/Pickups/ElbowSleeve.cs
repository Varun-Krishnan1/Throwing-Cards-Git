using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElbowSleeve : MonoBehaviour
{
    private bool triggered = false; 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && !triggered)
        {
            AudioManager.instance.Play("PickupItem");

            triggered = true; 

            col.gameObject.GetComponent<WeaponsManager>().ChangeFireSpeed(2);

            // -- display notification 
            Notifications.instance.AddNotification("Elbow Sleeve Found: Fire rate permanently increased"); 

            // -- destroy self after collected 
            Destroy(gameObject); 
        }
    }
}
