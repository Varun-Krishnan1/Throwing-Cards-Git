using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{

    [TextArea(3,10)]
    public string playbackSentence;
    public bool triggerAtTime; 
    public float waitTimeBeforePlaying;
    public GameObject[] onlyTriggerIfTheseTriggered;
    public GameObject[] onlyTriggerIfTheseNotTriggered; 

    private bool notHitPlayer = true; // -- have to add flag because capsule collider can contact twice! 
    private float timePassed = 0f;
    private bool triggerChecksPassed; 

    void Awake()
    {
        triggerChecksPassed = true; // -- assume everything triggered 
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // -- if it's not a trigger at time function 
        if(hitInfo.gameObject.tag == "Player" && notHitPlayer && !triggerAtTime && triggerChecksPassed)
        {
            notHitPlayer = false; 
            Dialogue.instance.AddSentence(playbackSentence);

            // -- destroy trigger so only executes once 
            Destroy(this.gameObject); 
        }
    }

    void Update()
    {
        triggerChecksPassed = true; 
        // -- iteratre through trigger objects array 
        foreach (GameObject triggerPoint in onlyTriggerIfTheseTriggered)
        {
            // -- if trigger point still exists in that array 
            if (triggerPoint != null)
            {
                ObjectTrigger objTrigger = triggerPoint.GetComponent<ObjectTrigger>();
                // -- if trigger point is an object trigger 
                if (objTrigger != null)
                {
                    // -- if it still hasn't been triggered 
                    if(!objTrigger.isTriggered())
                    {
                        triggerChecksPassed = false; // -- then don't let any functions run 
                    }
                }
                // -- if not trigger object then it's a trigger point and make sure it's destroyed 
                else
                {
                    triggerChecksPassed = false; // -- then don't let any functions run 
                }
            }
        }


        // -- if first array passes test 
        if(triggerChecksPassed)
        {
            // -- itearate through second trigger objects array 
            foreach (GameObject triggerPoint in onlyTriggerIfTheseNotTriggered)
            {
                // -- if trigger point still exists in that array 
                if (triggerPoint != null)
                {
                    ObjectTrigger objTrigger = triggerPoint.GetComponent<ObjectTrigger>();
                    // -- if trigger point is an object trigger 
                    if (objTrigger != null)
                    {
                        // -- if it HAS been triggered
                        if (objTrigger.isTriggered())
                        {
                            triggerChecksPassed = false; // -- then don't let any functions run 
                        }
                    }
                }
                // -- if object has been destroyed it's been triggered 
                else
                {
                    triggerChecksPassed = false; // -- then don't let any functions run 
                }
            }
        }

        // -- if it's a trigger by time dialogue and all trigger points checked 
        if (triggerAtTime && triggerChecksPassed)
        {
            // -- only then decrement time 
            waitTimeBeforePlaying -= Time.deltaTime; 

            // -- when timer expires 
            if(waitTimeBeforePlaying <= 0)
            {
                Dialogue.instance.AddSentence(playbackSentence);
                // -- destroy trigger so only executes once 
                Destroy(this.gameObject);
            }
        }
    }
}
