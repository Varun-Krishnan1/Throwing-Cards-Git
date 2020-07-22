using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using UnityEngine;



public class WeaponsManager : MonoBehaviour
{
    public Transform firePoint;
    public GameObject player;
    public Animator animator; 
    public Weapon[] weapons;
    public GameObject multiplierPopup;

    public float offset; 

    private int selectedWeaponIndex; 

    private bool cardsFrozen = true;

    void Start()
    {
        // -- start weapon on first weapon 
        selectedWeaponIndex = 0;
        Weapon weapon = weapons[selectedWeaponIndex];

        weapon.setWeaponImgObject(true);    // -- tell that weapons script to show the given weapon items 
    }

    void Update()
    {
        // -- gives player ability to freeze and unfreeze objects 
        FreezeAbility();

        // -- allow them to change speed pressing Q and E [TEMPORARY -> TO BECOME PICKED UP MULTIPLIER] 
        ChangeSpeed();

        // -- set active weapon 
        int previousSelectedWeapon = selectedWeaponIndex; 
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // -- cancel shooting animation and cane loading animation on weapon change 
            animator.SetBool("isShooting", false);
            animator.SetBool("isLoadingCane", false);

            if (selectedWeaponIndex >= 1)
            {
                selectedWeaponIndex = 0; 
            }
            else
            {
                selectedWeaponIndex++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // -- cancel shooting animation and cane loading animation on weapon change 
            animator.SetBool("isShooting", false);
            animator.SetBool("isLoadingCane", false);


            if (selectedWeaponIndex <= 0)
            {
                selectedWeaponIndex = 1;
            }
            else
            {
                selectedWeaponIndex--;
            }
        }

        // -- select weapon 
        if(previousSelectedWeapon != selectedWeaponIndex)
        {
            Weapon previousWeapon = weapons[previousSelectedWeapon];
            Weapon weapon = weapons[selectedWeaponIndex];

            previousWeapon.setWeaponImgObject(false);
            weapon.setWeaponImgObject(true);

        }

        // -- get input 
        if (Input.GetButtonDown("Fire1") && !animator.GetBool("isWallSliding") && !animator.GetBool("isRolling"))
        {
            Weapon weapon = weapons[selectedWeaponIndex];

            /* If on cane only throw when loaded */ 
            if(selectedWeaponIndex == 1)
            {
                CaneWeapon cane = (CaneWeapon)weapon; 

                // -- if cane not loaded then load cane 
                if (!cane.hasLoadedCane())
                {
                    cane.loadCane();
                    return;
                }
            }

            // -- rotate the firepoint to where they are pointing 
            setFirePointRotationBasedOnCursor();

            // - start the weapon shooting animation 
            weapon.startShootAnimation(); 

        }


    }

    // -- CALLED AT END OF WEAPON SHOOTING ANIMATION DO NOT CALL MANUALLY 
    public void EndAnimation()
    {
        animator.SetBool("isShooting", false);

    }


    // -- NEEDS REWORKING 
    public void ChangeSpeed()
    {
        // ------------ TESTING FIRE SPEED ----------------- [THIS NEEDS REWORKING] 
        if (Input.GetKeyDown("q"))
        {
            string printString = "";
            float curSpeed = animator.GetFloat("cardThrowingSpeed");

            if (curSpeed == 8)
            {
                // -- max speed reached 
                printString = "Max Speed Reached!";
            }
            else
            {
                foreach(Weapon weapon in weapons)
                {
                    weapon.setFireRateTime(weapon.getFireRateTime() / 2);

                }

                animator.SetFloat("cardThrowingSpeed", curSpeed * 2f);
                //float shootTime = animator.runtimeAnimatorController.animationClips.First(x => x.name == "CardThrowing").length;
                //fireRateTime = shootTimeFrac * curSpeed;  
                printString = (curSpeed * 2f).ToString() + "x";

            }
            MultiplierPopupController multiplierPopupCon = multiplierPopup.GetComponent<MultiplierPopupController>();
            multiplierPopupCon.Create(player.transform.position + new Vector3(0, 1.6f, 0), printString); ;


        }
        if (Input.GetKeyDown("e"))
        {
            string printString = "";
            float curSpeed = animator.GetFloat("cardThrowingSpeed");
            float shootTimeFrac = .62f;


            if (curSpeed == .125)
            {
                // -- slowest speed reached 
                printString = "Slowest Value Reached!";
            }
            else
            {

                foreach (Weapon weapon in weapons)
                {
                    weapon.setFireRateTime(weapon.getFireRateTime() * 2); 

                }

                animator.SetFloat("cardThrowingSpeed", curSpeed / 2f);
                //float shootTime = animator.runtimeAnimatorController.animationClips.First(x => x.name == "CardThrowing").length;
                //fireRateTime = shootTime * shootTimeFrac;
                printString = (curSpeed * .5f).ToString() + "x";

            }

            MultiplierPopupController multiplierPopupCon = multiplierPopup.GetComponent<MultiplierPopupController>();
            multiplierPopupCon.Create(player.transform.position + new Vector3(0, 1.6f, 0), printString);

        }
    }



    // -- CALLED DURING WEAPON SHOOTING ANIMATION DO NOT CALL MANUALLY 
    public void ShootWeapon()
    {
        Weapon weapon = weapons[selectedWeaponIndex];
        weapon.Shoot();
    }

    public void setFirePointRotationBasedOnCursor()
    {
        // -- get direction player is facing 
        Controller2D playerController = player.GetComponent<Controller2D>();
        bool facing_right = playerController.isFacingRight(); 

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position;

        float distance = 0;
        // -- get rotation based on difference between cursor position and PLAYER when cursor is closed to the player [This needs work ] 
        if (distance < 1)
        {
            distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), player.transform.Find("ControllerChecks/GroundCheck").transform.position);

        }
        // if not close get diff normally 
        
        {
            distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), firePoint.position);
        }
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        float firePointDirection = firePoint.transform.localEulerAngles.z;

        // -- flip player to correct position [This needs work ] 
        if (facing_right)
        {

            if (90 <= firePointDirection && firePointDirection <= 255)
            {
                playerController.Flip();
                firePoint.rotation = Quaternion.Euler(0f, 0f, firePointDirection);
            }
        }
        else
        {

            if (300 <= firePointDirection || firePointDirection <= 40)
            {
                playerController.Flip();
                firePoint.rotation = Quaternion.Euler(0f, 0f, firePointDirection);

            }
        }

    }

    private void FreezeAbility()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            GameObject[] thrownCards = GameObject.FindGameObjectsWithTag("ThrownPlayerCard");
            GameObject[] thrownCanes = GameObject.FindGameObjectsWithTag("ThrownCane");
            GameObject[] thrownObjects = thrownCards.Concat(thrownCanes).ToArray();
            // cardsFrozen starts as true 

            // if any objects are moving then just freeze those cards then exit 
            // spinning canes in middair are considered UNMOVING because they haven't hit object hence they won't trigger 
            // this to bool to be true! 
            bool anyCardsMoving = false;

            foreach (GameObject card in thrownObjects)
            {
                CardController cardController = card.GetComponent<CardController>();
                if (cardController.isMoving())
                {
                    cardController.Freeze();
                    anyCardsMoving = true;
                }
            }

            // if no objects moving then you can go and freeze and unfreeze moving cards and canes 
            if (!anyCardsMoving)
            {
                // -- If objects are frozen then UNFREEZE objects 
                if (cardsFrozen)
                {
                    foreach (GameObject card in thrownObjects)
                    {
                        /// -- unfreeze objects that are NOT moving 
                        CardController cardController = card.GetComponent<CardController>();
                        cardController.UnFreeze();
                    }
                    cardsFrozen = false;
                }
                // -- If objects are unfrozen then FREEZE objects 
                else
                {
                    foreach (GameObject card in thrownObjects)
                    {
                        // -- freeze objects that are NOT moving 
                        CardController cardController = card.GetComponent<CardController>();
                        cardController.Freeze();
                    }
                    cardsFrozen = true;
                }
            }
        }
    }
}
