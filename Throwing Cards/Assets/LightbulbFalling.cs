using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightbulbFalling : MonoBehaviour
{

    public Animator animator; 

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Floor")
        {
            animator.SetBool("hasBeenHit", true); 
        }
    }
}
