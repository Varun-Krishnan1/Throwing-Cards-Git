using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject player;
    public ContinueButton continueButton; 
    private PlayerData saveInfo; 
    void Awake()
    {
        saveInfo = GameManager.instance.GetSaveData();
        if (saveInfo != null)
        {
            continueButton.gameObject.SetActive(true); 
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }

    }

    public void NewGame()
    {
        // -- create save file with empty stats 
        player.SetActive(true); 
        GameManager.instance.SavePlayer(player);
        GameManager.instance.LoadNextScene(); 
    }

    public void ContinueGame()
    {
        int startLevel = saveInfo.level + 1;
        GameManager.instance.LoadScene(startLevel);
    }

    public void QuitGame()
    {

    }
}
