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
    public float wpm;
    public float timeOnScreenPerWord;
    public float extraTimeOnScreen; 
    public Animator textAnimator;
    public GameObject deathScreen; 


    private Queue<string> curSentences;
    private string curSentence = "";
    private float textTimer = 0f; 


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
        // -- get words 
        char[] delimiters = new char[] { ' ', '\r', '\n' };
        int numWords = curSentence.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

        // -- don't let them do new word yet 
        //textTimer = 1000f; 
        // -- assume 250 wpm -> 60/250 seconds per word then add the time it should remain on screen for 
        textTimer = (numWords * (60 / wpm)) + (timeOnScreenPerWord * numWords) + extraTimeOnScreen; 

        foreach (char letter in curSentence.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed); 
        }


    }

    void Update()
    {
        textTimer -= Time.deltaTime; 
        // -- HEARTBEAT ANIMATION WITH TEXT? 

        // -- only start typing next sentence if there is a next sentence 
        // -- and the last sentence has finished being displayed 



        if (curSentences.Count > 0 && textTimer <= 0)
        {
            // -- set global variable to next sentence 
            curSentence = curSentences.Dequeue();

            // -- clear text 
            textDisplay.text = "";

            // -- play text transition animation 
            textAnimator.SetBool("changeText", true);

            // -- actually type to screen 
            StartCoroutine(TypeText(curSentence)); 
        }
        else if(textTimer <= 0)
        {
            // -- clear text 
            textDisplay.text = "";
        }
    }

    public void AddSentence(string sentence)
    {
        curSentences.Enqueue(sentence); 
    }

    public void Death()
    {
        deathScreen.SetActive(true); 
    }

    public void Alive()
    {
        deathScreen.SetActive(false); 
    }

}
