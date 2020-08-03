using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollTutorial : TutorialCanvasController
{
    public Animator playerAnimator; 
    protected override void OnActionDestroy()
    {
        if (playerAnimator.GetBool("isRolling")) {
            Destroy(this.gameObject); 
        }
    }
}
