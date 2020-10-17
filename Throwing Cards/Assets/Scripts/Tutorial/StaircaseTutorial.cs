using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaircaseTutorial : MonoBehaviour
{
    public float waitTime;
    public GameObject tutImage;
    public GameObject tutText; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if(waitTime < 0)
        {
            this.tutImage.SetActive(true);
            this.tutText.SetActive(true); 
        }
    }
}
