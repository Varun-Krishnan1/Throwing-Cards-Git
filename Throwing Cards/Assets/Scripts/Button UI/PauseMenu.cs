using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool gameIsPaused = false;

    void Update()
    {
        // -- allow reloading of scene with r 
        if (Input.GetKeyDown("r"))
        {
            if(gameIsPaused)
            {
                Resume(); 
            }
            GameManager.instance.ReloadScene();
        }

        // -- allow for pausing with esc 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.instance.Play("PauseMenu"); 
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
                Resume(); 
                // -- go to start menu 
                GameManager.instance.LoadScene(0); 
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
