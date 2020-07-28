using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{

    public Gradient gradient;
    public Image fill;
    public Text healthCounter;
    public Slider slider; 


    private int maxHealth; 
    

    public void SetMaxHealth(int health)
    {
        maxHealth = health;

        slider.maxValue = health;
        this.SetHealth(maxHealth);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        healthCounter.text = health + "/" + maxHealth;


        // -- change health to gradient (normalized makes it b/w 0 and 1 )
        fill.color = gradient.Evaluate(slider.normalizedValue);
        healthCounter.color = gradient.Evaluate(slider.normalizedValue);
    }

    void Update()
    {
        // -- lock rotation 
        this.gameObject.transform.rotation = Quaternion.identity; 
        healthCounter.transform.rotation = Quaternion.identity;
    }
}
