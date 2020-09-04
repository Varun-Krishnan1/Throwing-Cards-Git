using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{

    public int health = 100;

    // Start is called before the first frame update
    void Awake()
    {
        Dialogue.instance.Alive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage; 
        if (health >= 0)
        {
            Die(); 
        }

    }

    void Die()
    {
        Dialogue.instance.Death();
        Destroy(gameObject); 
    }
}
