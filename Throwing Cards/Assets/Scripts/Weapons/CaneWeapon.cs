using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaneWeapon : Weapon
{
    public GameObject canePrefab;
    public Transform firePoint;
    public GameObject silkInHand; 
    public GameObject caneInHand;
    public Animator animator; 

    private bool caneIsLoaded = false; 
    // Start is called before the first frame update
    void Start()
    {
        caneInHand.SetActive(false);
    }

    // -- called to start shoot animation 
    public override bool startShootAnimation()
    {
        animator.SetBool("isShooting", true);
        return true; 
    }


    public override void Shoot()
    {
        GameObject newCane = Instantiate(canePrefab, firePoint.position, firePoint.rotation);

        // -- have to load new cane now 
        caneIsLoaded = false;
        caneInHand.SetActive(false);
        silkInHand.SetActive(true);
    }

    public override void setWeaponImgObject(bool activeState)
    {
        if(caneIsLoaded)
        {
            caneInHand.SetActive(activeState);
        }
        else
        {
            silkInHand.SetActive(activeState);
        }
    }

    public bool hasLoadedCane()
    {
        return caneIsLoaded; 
    }

    public void loadCane()
    {
        if(!caneIsLoaded)
        {
            animator.SetBool("isLoadingCane", true);
        }
    }

    // -- called when SilkToCane animation CLOSE to end 
    public void loadCaneEvent()
    {
        // -- once animations done deactivate what we don't need 
        caneInHand.SetActive(true);
        silkInHand.SetActive(false);

        caneIsLoaded = true;
        animator.SetBool("isLoadingCane", false);

    }

    public override float getFireRateTime()
    {
        // -- cane doesn't have a fire rate time 

        return 0f; 
    }

    public override void setFireRateTime(float value)
    {
        // -- cane doesn't have a fire rate time 
    }

}
