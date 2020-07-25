using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal; 

/* This class is inherited by CaneController! */ 
public class CardController : MonoBehaviour
{
    [Header("General")]
    public float speed = 20f;
    public int value = 10;
    public string suit;     // suit1 (diamond), suit2 (heart), suit3(spade), suit4(clubs)
    public float scaleFactor = .1f;
    public float enemyPauseTime;

    public Rigidbody2D rb;
    private BoxCollider2D col; 
    public GameObject damagePopup;

    [Header("Camera Shake")]
    public float cameraShakeStartIntensity = .5f;
    public float cameraShakeScaleFactor = .02f;
    public float cameraShakeDuration = .1f; 

    [Header("Lighting")]
    public Light2D light;
    public Color color1;
    public Color color2;
    public float lightScaleFactor = .05f;


    [Header("Particle Trail")]

    public GameObject particleTrail;
    public float particleTrailScaleFactor;
    public float particleTrailStartSize;         // to keep track of particle trail size 

    public Gradient gradient1;      // gradient for particle trails 
    public Gradient gradient2;

    [Header("Partice Impact Effect")]
    public GameObject particleEffect;       // particle impact effect 
    public float particleScaleFactor;
    public float particleEffectStartSize;       // particle impact effect start size 
    public Sprite[] suitSprites; 

    protected bool hitObject = false;
    protected bool cardFrozen = false;     // -- card starts as unfrozen when it's thrown only when it hits it's frozen  
    protected Vector3 colSize; 


    /* THIS IS OVERRIDEN BY CANE CONTROLLER SO WHATEVER YOU ADD HERE MAKE SURE TO ADD TO CANE CONTROLLER START METHOD */ 
    protected virtual void Start()          
    {
        rb.velocity = transform.right * speed;
        // -- get original collider size 
        col = this.GetComponent<BoxCollider2D>();
        colSize = col.size;

        // -- scale down size 
        col.size = new Vector3(.19f, .10f,1f);


        // -- set color of light and particle trail based on suit 
        if(suit == "Suit1" || suit == "Suit2")          // -- diamond and heart 
        {
            light.color = color1;
            var colLifetime = particleTrail.GetComponent<ParticleSystem>().colorOverLifetime;
            colLifetime.color = gradient1; 

        }
        else
        {
            light.color = color2;
            var colLifetime = particleTrail.GetComponent<ParticleSystem>().colorOverLifetime;
            colLifetime.color = gradient2;

        }


        // -- particle trail start size 
        ParticleSystem.MainModule psmain = particleTrail.GetComponent<ParticleSystem>().main;
        psmain.startSize = particleTrailStartSize;


    }

    private void setParticleImpactSprite()
    {
        // -- array that has sprite of suits that HAVE SAME NAME as suits in the playing cards folder 
        ParticleSystem particleEffectPS = particleEffect.GetComponent<ParticleSystem>(); 
        foreach(Sprite sprite in suitSprites)
        {
            if(sprite.name == this.suit) {
                particleEffectPS.textureSheetAnimation.SetSprite(0, sprite); 
            }
        }
    }


    /* OVERRIDEN BY CANE CONTROLLER */ 
    protected virtual void FixedUpdate()
    {
        if (!hitObject)
        {
            print("Scaling...");
            print(this.transform.localScale);
            // -- scale object and light until it hits something 
            this.transform.localScale += new Vector3(scaleFactor, scaleFactor, 0f);
            print(this.transform.localScale);

            light.pointLightOuterRadius += lightScaleFactor;

            // -- scale camera shake 
            cameraShakeStartIntensity += cameraShakeScaleFactor;

            // -- scale particle effect if not cane weapon 
            particleEffectStartSize += particleScaleFactor;

            // -- scale particle trail 
            ParticleSystem.MainModule psmain = particleTrail.GetComponent<ParticleSystem>().main;
            psmain.startSize = particleTrailStartSize + particleTrailScaleFactor;
            particleTrailStartSize = particleTrailStartSize + particleTrailScaleFactor;
        }

    }

    // -- changed from 
    protected virtual void OnCollisionEnter2D(Collision2D collisionObject)
    {
        GameObject hitInfo = collisionObject.gameObject;
        // -- get contact point 
        Vector3 contact = collisionObject.GetContact(0).point;

       
        EnemyController enemy = hitInfo.GetComponent<EnemyController>();

        // -- if hit enemy 
        if (enemy != null && !hitObject)
        {
            ScreenShakeAndParticleImpactEffect(contact, true); 

            // -- call enemy takedamage() function 
            enemy.TakeDamage(this.value, this.gameObject);


            // -- get the popup position of the collider it hits and put a popup of the damage there! 
            this.damagePopupEffect(hitInfo.gameObject.transform.Find("PopupPosition").position);        // -- this is a function in this class 

            // -- set global variable 
            hitObject = true;

        }

        // -- make object stick into object if it hits anything other than above if checks 
        else if (!hitObject)
        {
            // -- if hits interactable object add screen shake and particle impact effect 
            if (hitInfo.tag == "InteractableObject")
            {
                ScreenShakeAndParticleImpactEffect(contact, true); 
            }

            // -- stop particle trail 
            particleTrail.GetComponent<ParticleSystem>().Stop();

            // -- turn off card glow  
            light.intensity = 0f;

            objectStickingLogic(hitInfo, false); 
           

            // -- set global variables 
            hitObject = true;
            cardFrozen = true;
        }
    }

    protected void objectStickingLogic(GameObject hitInfo, bool stickToEnemies)
    {
        EnemyController enemy = hitInfo.gameObject.GetComponent<EnemyController>(); 
       
        if(stickToEnemies)
        {
            //this.transform.parent = hitInfo.transform.parent 
        }

        // -- if hits something with sprite renderer 
        if (hitInfo.GetComponent<SpriteRenderer>() != null)
        {
            // -- set sorting layer to one minus whatever it hits so illusion of stickign into object 
            int hitSortingLayer = hitInfo.GetComponent<SpriteRenderer>().sortingOrder;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = hitSortingLayer - 1;
        }

        gameObject.GetComponent<Animator>().enabled = false;

        BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();

        // -- change collider size back to original 
        col.size = colSize;

        // -- remove isTrigger after hitting an object and just make it a normal collider 
        //col.isTrigger = false;


        if (!stickToEnemies)
        {
            // -- don't let gravity and such affect it 
            this.Freeze();
        }

        // -- stop velocities and animation 
        rb.velocity = Vector2.zero;



        // -- let player interact with it 
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    private void ScreenShakeAndParticleImpactEffect(Vector3 contactPoint, bool destroyObject)
    {
        // -- set particle impact sprite here so delay doesn't happen 
        // -- in start() 
        setParticleImpactSprite();

        // -- split second pause for added effect 
        // -- camera shake and destroy object called in this function 
        StartCoroutine(pauseTime(enemyPauseTime, destroyObject));

        // -- particle impact effect at contact point 
        ParticleSystem.MainModule psmain = particleEffect.GetComponent<ParticleSystem>().main;
        psmain.startSize = particleEffectStartSize;
        Instantiate(particleEffect, contactPoint, transform.rotation);
    }


    // -- create a CardDamagePopup at a given position 
    /* THIS IS OVERRIDEN BY CANE CONTROLLER SO WHATEVER YOU ADD HERE MAKE SURE TO ADD TO CANE CONTROLLER METHOD */
    protected virtual void damagePopupEffect(Vector3 posit)
    {
        CardDamagePopupController damagePopupController = damagePopup.GetComponent<CardDamagePopupController>();
        damagePopupController.Create(posit, value, suit); 
    }

    public void toggleFreezeState()
    {
        if(cardFrozen)
        {
            UnFreeze(); 
        }
        else
        {
            Freeze(); 
        }

    }


    public void Freeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = true;
        cardFrozen = true;
    }

    public void UnFreeze()
    {
        //rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.None;
        cardFrozen = false;
    }


    public bool isMoving()
    {
        return !hitObject;
    }

    protected IEnumerator pauseTime(float duration, bool destroyObject)
    {
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;

        // -- camera shake 
        CameraShake.Instance.ShakeCamera(cameraShakeStartIntensity, cameraShakeDuration);

        if(destroyObject)
        {
            // -- NOW destroy object 
            Destroy(gameObject);
        }

    }

    public void changeMaterial(PhysicsMaterial2D mat)
    {
        col.sharedMaterial = mat;
    }

    // ---- Old Functions ---- 
    /* 
    public void FreezeIfNotMoving()
    {
        if (!isMoving())
        {
            Freeze(); 
        }
    }

    public void UnFreezeIfNotMoving()
    {
        if (!isMoving())
        {
            UnFreeze(); 
        }
    }
    */


}
