using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryAttackTutorial : TutorialCanvasController
{

    protected override void OnActionDestroy()
    {
        if (Input.GetButtonDown("Fire1")) {
            Destroy(this.gameObject); 
        }
    }

}
