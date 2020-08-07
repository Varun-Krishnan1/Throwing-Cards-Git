using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTutorial : TutorialCanvasController
{
    protected override void OnActionDestroy()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Destroy(this.gameObject); 
        }
    }
}
