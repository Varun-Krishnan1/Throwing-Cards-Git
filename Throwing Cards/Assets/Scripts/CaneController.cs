using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class CaneController : CardController 
{
    public float explosionTimer = 3f;
    public float explosionRadius = 2f;
    public float explosionForce = 10f; 

    private bool exploded = false; 

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

        if(!hitObject && !exploded)
        {
            // -- timer till it blows up 
            explosionTimer -= Time.deltaTime; 
            if(explosionTimer <= 0)
            {
                print("Boom!");
                exploded = true;
                Explode();
            }

}

    }

    public void Explode()
    {

    }
    // -- override CaneController damage popup words to cane's 
    protected override void damagePopupEffect(Vector3 posit)
    {
        CardDamagePopupController damagePopupController = damagePopup.GetComponent<CardDamagePopupController>();
        damagePopupController.Create(posit, this.value, "NA");
    }



}
