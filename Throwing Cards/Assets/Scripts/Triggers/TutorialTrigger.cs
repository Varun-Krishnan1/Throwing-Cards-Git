using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialCanvasController tutorial;

    private bool triggered = false; 

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && !triggered)
        {
            tutorial.gameObject.SetActive(true);
            tutorial.TriggerTutorial(col.gameObject);
            triggered = true; 
        }

    }
}
