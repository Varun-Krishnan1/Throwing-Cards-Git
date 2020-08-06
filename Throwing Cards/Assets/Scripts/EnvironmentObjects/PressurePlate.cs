using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public float pushForce;
    public float pushTimerTime;
    public float openTimerTime;
    public bool pushLeft = false; 
    public PressureDoor door; 
    private float pushTimer;
    private float openTimer; 

    void Awake()
    {
        if(pushLeft)
        {
            pushForce = -pushForce; 
        }
    }
    void FixedUpdate()
    {
        pushTimer -= Time.fixedDeltaTime;
        openTimer -= Time.fixedDeltaTime; 

        if(openTimer <= 0)
        {
            door.setDoorOpened(false);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Chain chain = collision.gameObject.GetComponent<Chain>(); 
        if(chain != null && pushTimer <= 0)
        {
            chain.rb.AddForce(new Vector2(pushForce, 0), ForceMode2D.Impulse);
            pushTimer = pushTimerTime;
            door.setDoorOpened(true);
            openTimer = openTimerTime; 
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Chain chain = collision.gameObject.GetComponent<Chain>();
        if (chain != null)
        {
            //door.setDoorOpened(false);
        }
    }

}
