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

    [SerializeField]
    List<string> scriptsOnLaunch;

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

        GameManager.ScriptManager.AddScriptRandomly(scriptsOnLaunch);
    }


    protected override void LookAtTarget()
    {
        Vector3 targetDir = guidedPosition - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if(angle > boresightAngle)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turningForce * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        Explode(20);
        
        DisableMissile();
    }

    protected override void DisableMissile()
    {
        SetMissionStatus(isHit);
        
        // Send Message to object that it is no more locked on
        if(isHit == false)
        {
            ShowMissedLabel();
        }
        else
        {
            if(gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Destroyed);
            }
        }
        
        laserGuidanceController.HideGuidanceUI();
        isDisabled = true;
        transform.parent = parent;
        gameObject.SetActive(false);
    }
    
    void SetMissionStatus(bool hit)
    {
        MissionMaverick mission = (MissionMaverick)GameManager.MissionManager;

        mission.CheckBombing(hit);
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
