using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public float fireRateMultiplier;
    public bool hasCane; 

    public PlayerData (GameObject player)
    {

        // -- get data from game manager 
        level = GameManager.instance.GetLevelNumber();

        // -- get data from player 
        WeaponsManager weaponsManager = player.GetComponent<WeaponsManager>();
        fireRateMultiplier = weaponsManager.getFireRate();
        hasCane = weaponsManager.hasCaneInWeapons(); 

    }

    public static void LoadPlayerData(GameObject player)
    {
        PlayerData data = SaveSystem.LoadPlayer();

        // -- set player variables 
        player.GetComponent<WeaponsManager>().ChangeFireSpeed(data.fireRateMultiplier);
        if(data.hasCane)
        {
            player.GetComponent<WeaponsManager>().AddWeapon("SilkToCane");
        }
    }
}
