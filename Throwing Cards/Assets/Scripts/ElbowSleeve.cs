using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElbowSleeve : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<WeaponsManager>().ChangeFireSpeed(2);

            // -- display notification 
            Notifications.instance.AddNotification("Fire rate permanently increased"); 

            // -- destroy self after collected 
            Destroy(gameObject); 
        }
    }
}
