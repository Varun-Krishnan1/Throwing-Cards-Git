﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // -- prevent flipping of canvas 
        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
