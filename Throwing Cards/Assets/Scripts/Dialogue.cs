using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Dialogue : MonoBehaviour
{
    public static Dialogue instance = null; 
    public TextMeshProUGUI textDisplay;
    public float typingSpeed;
    public Animator textAnimator; 

    private Queue<string> curSentences;
    private string curSentence = ""; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // -- clear text 
        textDisplay.text = "";

        curSentences = new Queue<string>(); 

    }

    IEnumerator TypeText(string curSentence)
    {
        foreach (char letter in curSentence.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed); 
        }

        // -- after certain seconds remove text 
        yield return new WaitForSeconds(2f);
        textDisplay.text = "";

        // -- make sure to reset global variable so update can be called again 
        curSentence = ""; 
    }

    void Update()
    {

        // -- HEARTBEAT ANIMATION WITH TEXT? 

        // -- only start typing next sentence if there is a next sentence 
        // -- and the last sentence has finished being displayed 
        if(curSentences.Count > 0 && textDisplay.text == curSentence)
        {
            print("Dialogue Update Called...");
            // -- set global variable to next sentence 
            curSentence = curSentences.Dequeue();

            // -- clear text 
            textDisplay.text = "";

            // -- play text transition animation 
            textAnimator.SetBool("changeText", true);

            // -- actually type to screen 
            StartCoroutine(TypeText(curSentence)); 
        }
    }

    public void AddSentence(string sentence)
    {
        print("Adding sentence...");
        curSentences.Enqueue(sentence); 
    }

}
