using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// -- BASE CLASS ALL TUTORIAL POPUPS WILL DERIVE FROM! 
public abstract class TutorialCanvasController : MonoBehaviour
{
    //public float followSpeed;
    public string text;
    public TextMeshProUGUI textDisplay;


    protected bool triggered;
    //protected Transform followTarget; 
    void Awake()
    {
        textDisplay.text = text; 
    }

    protected void LateUpdate()
    {
        if(triggered)
        {
            //transform.position = Vector3.MoveTowards(transform.position, followTarget.position, followSpeed);
            OnActionDestroy(); 
        }
    }
    
    public void TriggerTutorial(GameObject player)
    {
        this.triggered = true;
        //this.followTarget = player.transform.Find("TutorialFollowPosition"); 
    }

    // -- unique to that tutorial but on what action should this tutorial object be destroyed 
    protected abstract void OnActionDestroy(); 
}
