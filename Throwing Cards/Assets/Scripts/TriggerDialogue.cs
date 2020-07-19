using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{

    [TextArea(3,10)]
    public string playbackSentence; 
    private bool notHitPlayer = true; // -- have to add flag because capsule collider can contact twice! 
    
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player" && notHitPlayer)
        {
            notHitPlayer = false; 
            Dialogue.instance.AddSentence(playbackSentence);

            // -- destroy trigger so only executes once 
            Destroy(this.gameObject); 
        }
    }
}
