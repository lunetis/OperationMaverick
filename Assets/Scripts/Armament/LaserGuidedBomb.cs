using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGuidedBomb : Missile
{
    // Guidance
    public Vector3 guidedPosition;
    public float initialFallAmount = 4;
    public float lerpAmount = 1;

    LaserGuidanceController laserGuidanceController;

    public override void Launch(Vector3 guidedPosition, float launchSpeed, int layer, GameObject launcher)
    {
        minimapSprite.SetMinimapSpriteVisible(true);
        isDisabled = false;

        this.guidedPosition = guidedPosition;
        speed = launchSpeed;
        gameObject.layer = layer;

        laserGuidanceController = launcher.GetComponent<LaserGuidanceController>();
        laserGuidanceController.lgb = this;

        laserGuidanceController.ShowGuidanceUI();
    }


    protected override void LookAtTarget()
    {
        Vector3 targetDir = guidedPosition - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turningForce * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        // This line must be collision check
        if(true)
        {
            isHit = true;
        }

        Explode();
        DisableMissile();
    }

    protected override void DisableMissile()
    {
        // Send Message to object that it is no more locked on
        if(isDisabled == false && isHit == false)
        {
            ShowMissedLabel();
        }
        
        laserGuidanceController.HideGuidanceUI();
        isDisabled = true;
        transform.parent = parent;
        gameObject.SetActive(false);
    }

    protected override void AdjustValuesByDifficulty()
    {
        
    }

    void OnDisable()
    {    
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        LookAtTarget();
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        initialFallAmount = Mathf.Lerp(initialFallAmount, 0, lerpAmount * Time.deltaTime);
        transform.Translate(Vector3.down * initialFallAmount * Time.deltaTime);
    }
}
