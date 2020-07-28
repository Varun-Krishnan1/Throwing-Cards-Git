using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraRotation : MonoBehaviour
{

    public float offset;
    public float cutOffAngle;

    private GameObject player; 

    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");    
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(angle > cutOffAngle)
        {
            transform.rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        }
    }
}
