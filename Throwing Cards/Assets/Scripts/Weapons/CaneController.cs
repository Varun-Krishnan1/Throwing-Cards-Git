using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class CaneController : CardController 
{
    public Animator animator; 
    [Header("Explosion")]
    public float explosionTimer = 3f;
    public float explosionRadius = 2f;
    public float explosionForce = 10f;
    public LayerMask layersToHit;
    public ParticleSystem explosionEffect;
    public Light2D caneLight; 


    private bool exploded = false;
    private bool hitEnemy = false;
    private Transform enemyHit; 

    // Start is called before the first frame update
    // -- base's start method not called at all now 
    protected override void Start()
    {
        rb.velocity = transform.right * speed;
        
        
        // -- get original collider size 
        BoxCollider2D col = this.GetComponent<BoxCollider2D>();
        colSize = col.size;

        // -- DO NOT scale down size till we figure out correct scaled down size for cane 
        //col.size = new Vector3(.22f, .13f, 1f);

        base.colSize = colSize;

        // -- particle trail 
        var colLifetime = particleTrail.GetComponent<ParticleSystem>().colorOverLifetime;
        colLifetime.color = gradient1; 

        ParticleSystem.MainModule psmain = particleTrail.GetComponent<ParticleSystem>().main;
        psmain.startSize = particleTrailStartSize;
    }

    protected override void FixedUpdate()
    {
        // -- only scale if cane is not frozen or hasn't hit anything 
        if (!hitObject && !cardFrozen && !exploded)
        {
            // -- scale object and light until it hits something 
            // -- CAN'T SCALE BECAUSE ANIMATOR DOESN'T ALLOW IT 
            this.transform.localScale += new Vector3(scaleFactor, scaleFactor, 0f);
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

    protected override void OnCollisionEnter2D(Collision2D collisionObject)
    {
        Vector3 contact = collisionObject.GetContact(0).point;

        if (!hitObject && !exploded)
        {
            // -- stick into objects on collisiosn 
            objectStickingLogic(collisionObject.gameObject, true);      // -- same method that card controller uses 

            if(collisionObject.gameObject.tag == "Enemy")
            {
                hitEnemy = true;
                enemyHit = collisionObject.gameObject.transform.Find("PopupPosition"); 
            }

            hitObject = true; 
        }

    }

    void Update()
    {
        if(hitEnemy && enemyHit != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, enemyHit.position, .1f);
        }

        // -- timer till it blows up 
        explosionTimer -= Time.deltaTime;
       
        if (explosionTimer <= 0 && !exploded)
        {
            // -- re-enable animation in case it was frozen  
            // -- gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;

            //animator.SetBool("isExploding", true);

            // -- hide cane sprite and turn off light 
            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
            caneLight.enabled = false; 

            explosionEffect.Play();

            var main = explosionEffect.main;

            this.Freeze();
            
            Explode();

            exploded = true;


            // -- destroy object after particle system effect ends 
            Invoke("EndAnimation", main.duration);

        }
    }

    public void EndAnimation()
    {
        Destroy(gameObject); 
    }

    public void Explode()
    {
        // -- screen shake but don't destroy object 
        StartCoroutine(pauseTime(enemyPauseTime, false));

        Collider2D[] objects = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius, layersToHit); 
        
        foreach(Collider2D obj in objects)
        {
            Vector2 direction = obj.transform.position - transform.position;

            // -- add force to objects 
            Rigidbody2D rb = obj.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null && obj.gameObject != gameObject && obj.gameObject.tag != "RoomGrid")
            {
               // rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);

                // -- do damage to enemies 
                EnemyController enemy = obj.gameObject.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(this.value, this.gameObject);


                    // -- get the popup position of the collider it hits and put a popup of the damage there! 
                    this.damagePopupEffect(obj.gameObject.transform.Find("PopupPosition").position);        // -- this is a function in this class 
                }

            }
        }

        AudioManager.instance.Play("Explosion"); 

    }

    // -- override CaneController damage popup words to cane's 
    protected override void damagePopupEffect(Vector3 posit)
    {
        CardDamagePopupController damagePopupController = damagePopup.GetComponent<CardDamagePopupController>();
        damagePopupController.Create(posit, this.value, "NA");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); 
    }



}
