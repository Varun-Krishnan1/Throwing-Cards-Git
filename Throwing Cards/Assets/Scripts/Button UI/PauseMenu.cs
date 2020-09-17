using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool gameIsPaused = false;

    private string cheatWord = "";

    void Update()
    {
        // -- allow reloading of scene with r 
        if (Input.GetKeyDown("r"))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            GameManager.instance.ReloadScene();
        }

        // -- allow for pausing with esc 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.instance.Play("PauseMenu");
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // -- allow for quitting with q 
        if (gameIsPaused)
        {
            if (Input.GetKeyDown("q"))
            {
                Resume();
                // -- go to start menu 
                GameManager.instance.LoadScene(0);
            }
        }

        // -- allow skipping of level with Lea 
        if (Input.GetKeyDown("l") && cheatWord == "")
        {
            cheatWord = "l";
        }
        else if (Input.GetKeyDown("e") && cheatWord == "l")
        {
            cheatWord += "e";
        }
        else if(Input.GetKeyDown("a") && cheatWord == "le")
        {
            // -- CHEAT WORD WRITTEN! 
            GameManager.instance.SavePlayer(GameObject.FindWithTag("Player"));
            GameManager.instance.LoadNextScene();
        }
        print(cheatWord); 
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

        // -- reset cheat word on pause 
        cheatWord = ""; 
    }
}
