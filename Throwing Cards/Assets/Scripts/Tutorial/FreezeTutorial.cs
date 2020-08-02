﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTutorial : TutorialCanvasController
{
    public GameObject freezeDisplay;
    public GameObject unFreezeDisplay; 

    private bool cardsFrozen = true; 
    protected override void OnActionDestroy()
    {
        if (Input.GetButtonDown("Fire2"))
        { 
            cardsFrozen = !cardsFrozen; 
        }


        if (cardsFrozen)
        {
            unFreezeDisplay.SetActive(true);
            freezeDisplay.SetActive(false);
        }
        else
        {
            unFreezeDisplay.SetActive(false);
            freezeDisplay.SetActive(true);
        }
    }
}
 