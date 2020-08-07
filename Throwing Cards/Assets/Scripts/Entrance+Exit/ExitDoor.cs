using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public bool horizontal; 
    public int numKeysRequired = 1;
    public float checkRadius = 1f;
    public float yRange;
    public float moveUpSpeed; 
    public Transform checkPosition; 
    public LayerMask playerMask;
    public GameObject keyNotification;


    private ExitController exitController; 
    private Vector3 original; 
    private int numKeys; 

    private bool originalObtained; 

    void Awake()
    {
        exitController = (ExitController)FindObjectOfType(typeof(ExitController));
    }
    void Update()
    {
        if(!originalObtained)
        {
            original = this.transform.position;
            originalObtained = true; 
        }
        /* If player in circle radius and number of keys gotten on map is equal to keys required */
        if (Physics2D.OverlapCircle(checkPosition.position, checkRadius, playerMask))
        {
            if (numKeys == numKeysRequired)
            {
                MoveDoor(moveUpSpeed); 
                keyNotification.SetActive(false);

            }
            else
            {
                MoveDoor(-moveUpSpeed); 
                keyNotification.SetActive(true); 
            }
        }
        else
        {
            MoveDoor(-moveUpSpeed); 
            keyNotification.SetActive(false);
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
            if(horizontal)
            {
                 if(after >= originalPos)
                 {
                    AudioManager.instance.Play("DoorOpen", true);
                    gameObject.transform.position = new Vector3(after, gameObject.transform.position.y, gameObject.transform.position.z);
                    return; 
                 }
            }
            else
            {
                if (after <= originalPos)
                {
                    AudioManager.instance.Play("DoorOpen", true);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, after, gameObject.transform.position.z);
                    return; 
                }
            }
        }

        //AudioManager.instance.Stop("DoorOpen");
    }

    /* Called by Key */
    public void AddKey()
    {
        this.numKeys += 1;
    }

}
