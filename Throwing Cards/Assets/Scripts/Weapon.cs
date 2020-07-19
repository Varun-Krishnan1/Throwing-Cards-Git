using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -- BASE CLASS ALL WEAPONS INHERIT FROM 
public abstract class Weapon : MonoBehaviour
{
    public abstract void Shoot();

    public abstract void setWeaponImgObject(bool activeState);

    // -- returns if animation started or if it didn't(doesn't if fire rate is reloading) 
    public abstract bool startShootAnimation();
}
