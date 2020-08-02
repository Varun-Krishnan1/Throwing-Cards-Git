using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    public float checkRadius = 2f;
    public Transform checkPosition;
    public LayerMask playerMask;
    public Key key;
    public GameObject keyText; 

    public GameObject getToDoorText;

    private bool wentToDoor; 
    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(checkPosition.position, checkRadius, playerMask) && !wentToDoor)
        {
            key.gameObject.SetActive(true);
            keyText.SetActive(true); 
            getToDoorText.SetActive(false);
            wentToDoor = true; 
        }
    }
}
