using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public ExitDoor door;

    private bool triggered = false; 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && !triggered)
        {
            triggered = true;

            door.AddKey();
            Destroy(gameObject);
        }
    }
}
