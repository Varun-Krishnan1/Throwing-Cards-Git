using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject audioManager; 
    // Called at beggining of game to create a game manager 
    void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
        //if(AudioManager.instance == null)
        //{
        //    Instantiate(audioManager); 
        //}
    }

}
