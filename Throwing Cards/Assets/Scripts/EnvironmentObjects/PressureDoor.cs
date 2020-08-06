using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureDoor : MonoBehaviour
{
    public bool horizontal = false;
    public float yRange;
    public float moveSpeed; 

    private bool originalObtained = false;
    private bool doorOpened = false;
    private Vector3 original; 
    
    void Update()
    {
        if (!originalObtained)
        {
            original = this.transform.position;
            originalObtained = true;
        }

        if(doorOpened)
        {
            MoveDoor(moveSpeed); 
        }
        else
        {
            MoveDoor(-moveSpeed); 
        }
    }

    private void MoveDoor(float moveSpeed)
    {
        float after = gameObject.transform.position.y + moveSpeed;
        float originalPos = original.y;

        if (horizontal)
        {
            // -- flip movespeed for horizontal because positive x closes door 
            after = gameObject.transform.position.x + -moveSpeed;
            originalPos = original.x;
        }

        if (Mathf.Abs(originalPos - after) <= yRange)
        {
            if (horizontal)
            {
                if (after >= originalPos)
                {
                    gameObject.transform.position = new Vector3(after, gameObject.transform.position.y, gameObject.transform.position.z);
                }
            }
            else
            {
                if (after <= originalPos)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, after, gameObject.transform.position.z);
                }
            }
        }
    }

    public void setDoorOpened(bool opened)
    {
        this.doorOpened = opened; 
    }
}
