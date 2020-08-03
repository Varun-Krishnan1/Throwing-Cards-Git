using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleTutorial : TutorialCanvasController
{
    public EnemyController enemy; 
    protected override void OnActionDestroy()
    {
        if (enemy == null)
        {
            Destroy(this.gameObject);
        }
    }
}
