using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    private GameObject pauseMenu;


    void Awake()
    {
        pauseMenu = GameObject.FindWithTag("MainCanvas"); 
    }


    void Update()
    {
        // -- allow reloading of scene with r 
        if (Input.GetKeyDown("r"))
        {
            GameManager.instance.ReloadScene();
        }

        // -- allow for pausing with esc 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
            {
                Resume(); 
            }
            else
            {
                Pause(); 
            }
        }
        
        // -- allow for quitting with q 
        if(gameIsPaused)
        {
            if (Input.GetKeyDown("q"))
            {
                Application.Quit(); 
            }
        }
    }

    void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true; 
    }
}
