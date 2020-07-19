using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{

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
        print(SceneManager.GetActiveScene().name);
        print(level);

    }

    public enum Scene
    {
        Level0, Level1, Level2, Level3
    }


    // -- pass something in like GameManager.instance.LoadScene(GameManager.Scene.SampleScene); 
    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());

    }


    public void LoadNextScene()
    {
        level += 1;
        print(level);
        // -- assumes order is based on order in the Scene enum 
        SceneManager.LoadScene(sceneArr.GetValue(level).ToString());
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
