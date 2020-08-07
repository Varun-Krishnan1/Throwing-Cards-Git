using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{

    // -- for singleton pattern 
    public static GameManager instance = null;

    private Array sceneArr; 
    private int level = 0; 
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this; 
        }
        else if(instance != this)
        {
            Destroy(gameObject); 
        }

        DontDestroyOnLoad(gameObject);

        sceneArr = Enum.GetValues(typeof(Scene));

        Scene curScene = (Scene) Enum.Parse(typeof(Scene), SceneManager.GetActiveScene().name);
        level = Array.IndexOf(sceneArr, curScene);

    }

    public enum Scene
    {
        StartMenu, Level0, Level1, Level2, Level3, Level5, Level6, Level7, Level8, Level20, Level70 
    }


    // -- pass something in like GameManager.instance.LoadScene(GameManager.Scene.SampleScene); 
    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());

    }

    public void LoadScene(int level)
    {
        this.level = level; 
        SceneManager.LoadScene(sceneArr.GetValue(level).ToString());
    }

    // -- called by exit controller before SetActive is false for player 
    public void SavePlayer(GameObject player)
    {
        // -- calls constructor in PlayerData 
        SaveSystem.SavePlayer(player);
    }

    // -- called by entrance controller 
    public void LoadPlayer(GameObject player)
    {
        PlayerData.LoadPlayerData(player); 
    }

    public void LoadNextScene()
    {
        level += 1;
        // -- assumes order is based on order in the Scene enum 
        this.LoadScene(level);
    }

    public PlayerData GetSaveData()
    {
        return SaveSystem.LoadPlayer(); 
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetLevelNumber()
    {
        return this.level; 
    }

}
