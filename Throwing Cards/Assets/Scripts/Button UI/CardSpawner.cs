using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject player; 
    public float fireRateTime;
    public GameObject fancyCard; 
    private float fireRate; 

    void Update()
    {
        fireRate -= Time.deltaTime; 
        if(fireRate <= 0)
        {
            // -- must be outside of range (0.0, 1
            Vector3 v3Pos = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(-.5f,-1f), UnityEngine.Random.Range(0f,1f), 10.0f));
            player.transform.position = v3Pos;
            player.GetComponent<CardWeapon>().ShootStartMenu(fancyCard); 
            fireRate = fireRateTime; 
        }
    }

}
