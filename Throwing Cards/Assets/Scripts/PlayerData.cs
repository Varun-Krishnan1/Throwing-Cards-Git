using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public float fireRateMultiplier; 

    public PlayerData (GameObject player)
    {

        // -- get data from game manager 
        level = GameManager.instance.GetLevelNumber();

        // -- get data from player 
        fireRateMultiplier = player.GetComponent<WeaponsManager>().getFireRate();

    }

    public static void LoadPlayerData(GameObject player)
    {
        PlayerData data = SaveSystem.LoadPlayer();

        // -- set player variables 
        player.GetComponent<WeaponsManager>().ChangeFireSpeed(data.fireRateMultiplier);
    }
}
