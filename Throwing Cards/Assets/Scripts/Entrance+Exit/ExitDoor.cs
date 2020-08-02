using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public int numKeysRequired = 1;
    public float checkRadius = 1f;
    public Transform checkPosition; 
    public LayerMask playerMask;
    public Animator animator;
    public GameObject keyNotification; 
    private ExitController exitController; 

    private int numKeys; 

    void Awake()
    {
        exitController = (ExitController)FindObjectOfType(typeof(ExitController));
    }
    void Update()
    {
        /* If player in circle radius and number of keys gotten on map is equal to keys required */
        if (Physics2D.OverlapCircle(checkPosition.position, checkRadius, playerMask))
        {
            if (numKeys == numKeysRequired)
            {
                animator.SetBool("doorOpening", true);
                keyNotification.SetActive(false);

            }
            else
            {
                animator.SetBool("doorOpening", false);
                keyNotification.SetActive(true); 
            }
        }
        else
        {
            animator.SetBool("doorOpening", false);
            keyNotification.SetActive(false);
        }

    }

    /* Called by Key */ 
    public void AddKey()
    {
        this.numKeys += 1;
    }

    public void GoNextLevel()
    {
        exitController.GoNextLevel(); 
    }
}
