using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Notifications : MonoBehaviour
{
    public static Notifications instance = null;
    public TextMeshProUGUI notificationDisplay;
    public Animator textAnimator;
    public float wpm;
    public float timeOnScreenPerWord;
    public float extraTimeOnScreen;

    private Queue<string> curNotifications;
    private string curNotification = "";
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
        notificationDisplay.text = "";

        curNotifications = new Queue<string>();

    }

    void DisplayNotification()
    {
        char[] delimiters = new char[] { ' ', '\r', '\n' };
        int numWords = curNotification.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

        textTimer = (numWords * (60 / wpm)) + (timeOnScreenPerWord * numWords) + extraTimeOnScreen;

        notificationDisplay.text = curNotification; 
    }

    void Update()
    {
        textTimer -= Time.deltaTime;

        // -- only start typing next sentence if there is a next sentence 
        // -- and the last sentence has finished being displayed 



        if (curNotifications.Count > 0 && textTimer <= 0)
        {
            // -- set global variable to next sentence 
            curNotification = curNotifications.Dequeue();

            // -- clear text 
            notificationDisplay.text = "";

            // -- play text transition animation 
            textAnimator.SetBool("changeText", true);

            // -- actually type to screen 
            DisplayNotification(); 
        }
        else if (textTimer <= 0)
        {
            // -- clear text 
            notificationDisplay.text = "";
        }


    }

    public void AddNotification(string sentence)
    {
        curNotifications.Enqueue(sentence);
    }

}
