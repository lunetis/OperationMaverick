using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AircraftController))]

public class WeaponController : MonoBehaviour
{
    // Weapon Inputs
    bool useSpecialWeapon;

    // Weapon System
    [Header("Common Weapon System")]
    TargetObject target;

    [SerializeField]
    float targetDetectDistance;

    [SerializeField]
    Transform leftMissileTransform;
    [SerializeField]
    Transform rightMissileTransform;
    [SerializeField]
    Transform centerMissileTransform;

    // Missile
    [Header("Missile")]
    [SerializeField]
    Missile missile;
    WeaponSlot[] mslSlots = new WeaponSlot[2];

    ObjectPool missilePool;
    int missileCnt;
    float missileCooldownTime;

    // Special Weapon;
    [Header("Special Weapon")]
    [SerializeField]
    Missile specialWeapon;
    WeaponSlot[] spwSlots = new WeaponSlot[2];

    ObjectPool specialWeaponPool;
    string specialWeaponName;
    int specialWeaponCnt;
    float spwCooldownTime;
    
    public MonoBehaviour specialWeaponScript;
    
    // Machine Gun
    [Header("Machine Gun")]
    [SerializeField]
    int bulletCnt;
    [SerializeField]
    Transform gunTransform;
    [SerializeField]
    float gunRPM;
    [SerializeField]
    float vibrateAmount;

    ObjectPool bulletPool;
    float fireInterval;
    
    bool isFocusingTarget;

    // UI / Misc
    [Header("UI / Misc.")]
    [SerializeField]
    MinimapController minimapController;

    [SerializeField]
    GunCrosshair gunCrosshair;

    [Header("Sounds")]
    [SerializeField]
    AudioClip ammunitionZeroClip;
    [SerializeField]
    AudioClip cooldownClip;

    [SerializeField]
    AudioSource voiceAudioSource;
    [SerializeField]
    AudioSource weaponAudioSource;
    [SerializeField]
    AudioSource missileAudioSource;

    [SerializeField]
    GunAudio gunAudio;

    AircraftController aircraftController;
    UIController uiController;

    TargetObject nextTarget;

    Vector3 prevDirectionAtTargetChange;
    float prevTargetChangeTime;

    [SerializeField]
    float targetChangeAngleThreshold = 90;
    [SerializeField]
    float targetChangeTimeThreshold = 5;


    public Transform GunTransform
    {
        get { return gunTransform; }
    }

    public int SpecialWeaponCnt
    {
        set
        {
            specialWeaponCnt = value;
            GameManager.UIController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
        }
    }


    // Weapon Callbacks
    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            if(useSpecialWeapon == true)
            {
                LaunchMissile(ref specialWeaponCnt, ref specialWeaponPool, ref spwSlots);
            }
            else
            {
                LaunchMissile(ref missileCnt, ref missilePool, ref mslSlots);
            }
        }
    }

    public void OnGunFire(InputAction.CallbackContext context)
    {
        switch(context.action.phase)
        {
            case InputActionPhase.Started:
                InvokeRepeating("FireMachineGun", 0, fireInterval);
                gunAudio.IsFiring = true;
                GameManager.GamepadController.isGunFiring = true;
                break;

            case InputActionPhase.Canceled:
                CancelInvoke("FireMachineGun");
                gunAudio.IsFiring = false;
                GameManager.GamepadController.isGunFiring = false;
                break;
        }
    }

    public void OnChangeTarget(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            isFocusingTarget = false;
        }

        // Hold Interaction Performed (0.3s)
        else if(context.action.phase == InputActionPhase.Performed)
        {
            if(target == null) return;

            GameManager.CameraController.LockOnTarget(target.transform);
            isFocusingTarget = true;
        }
        
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            // Hold : Focus
            if(isFocusingTarget == true)
            {
                GameManager.CameraController.LockOnTarget(null);
            }
            // Press : Change Target
            else
            {
                ChangeTarget();
            }
        }
    }

    public void NotifyTargetDestroy(TargetObject destroyedTarget)
    {
        if(target == destroyedTarget)
        {
            ChangeTarget();
        }
    }

    public void ChangeTarget()
    {
        TargetObject newTarget = GetNextTarget();
        if(newTarget == null)   // No target
        {
            GameManager.CameraController.LockOnTarget(null);
            GameManager.TargetController.SetTarget(null);
            gunCrosshair.SetTarget(null);
            target = null;
            
            return;
        }

        // No change
        if(newTarget == target) return;

        // Previous Target
        if(target != null)
        {
            target.SetMinimapSpriteBlink(false);
        }

        target = newTarget;
        target.isNextTarget = false;
        target.SetMinimapSpriteBlink(true);
        GameManager.TargetController.SetTarget(target);
        gunCrosshair.SetTarget(target.transform);
    }

    TargetObject GetNextTarget()
    {
        List<TargetObject> targets = GameManager.Instance.GetTargetsWithinDistance(5000);
        TargetObject selectedTarget = null;

        if(targets.Count == 0)
        {
            nextTarget = null;
            return null;
        }

        else if(targets.Count == 1)
        {
            selectedTarget = targets[0];
            nextTarget = null;
            return selectedTarget;
        }
            
        else
        {
            // Reset next target indicator
            for(int i = 0; i < targets.Count; i++)
            {
                targets[i].isNextTarget = false;
            }
                
            for(int i = 0; i < targets.Count; i++)
            {
                // No current target
                if(target == null)
                {
                    selectedTarget = targets[0];   // not selected
                    nextTarget = targets[1];
                    break;
                }

                // There's a next target in the array
                if(targets[i] == nextTarget)
                {
                    // Set current target to next target
                    selectedTarget = nextTarget;

                    if(i == targets.Count - 1)  // if current target is the last index
                    {
                        nextTarget = targets[0];    // then next target is the first index
                    }
                    else
                    {
                        // Force change when index > 1
                        if(i > 0 && 
                            (Vector3.Angle(transform.forward, prevDirectionAtTargetChange) > targetChangeAngleThreshold) ||
                            (Time.time - prevTargetChangeTime > targetChangeTimeThreshold))
                        {
                            prevDirectionAtTargetChange = transform.forward;
                            nextTarget = targets[0];
                        }
                        else
                        {
                            nextTarget = targets[i + 1];
                        }
                    }
                    break;
                }
            }

            // There is no current target in the array
            if(selectedTarget == null)
            {
                selectedTarget = targets[0];   // not selected
                nextTarget = targets[1];
            }
        }
        nextTarget.isNextTarget = true;
        prevTargetChangeTime = Time.time;

        return selectedTarget;
    }


    void FireMachineGun()
    {
        if(bulletCnt <= 0)
        {
            // Beep sound
            CancelInvoke("FireMachineGun");
            return;
        }

        GameObject bullet = bulletPool.GetPooledObject();
        bullet.transform.position = gunTransform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.SetActive(true);

        TargetObject reservedHitTargetObject = gunCrosshair.CheckGunHit();

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Fire(aircraftController.Speed, gameObject.layer, reservedHitTargetObject);
        bulletCnt--;
        
        // Vibration
        uiController.SetGunText(bulletCnt);
    }

    // Return the oldest-launched WeaponSlot
    // If none of them are available, return null
    WeaponSlot GetAvailableWeaponSlot(ref WeaponSlot[] weaponSlots)
    {
        WeaponSlot oldestSlot = null;

        foreach(WeaponSlot slot in weaponSlots)
        {
            if(slot.IsAvailable() == true)
            {
                if(oldestSlot == null)
                {
                    oldestSlot = slot;
                }
                else if(oldestSlot.LastStartCooldownTime > slot.LastStartCooldownTime)
                {
                    oldestSlot = slot;
                }
            }
        }

        return oldestSlot;
    }


    void LaunchMissile(ref int weaponCnt, ref ObjectPool objectPool, ref WeaponSlot[] weaponSlots)
    {
        WeaponSlot availableWeaponSlot = GetAvailableWeaponSlot(ref weaponSlots);
        
        // Ammunition Zero!
        if(weaponCnt <= 0)
        {
            int activeChildCnt = 0;
            foreach(Transform child in objectPool.transform)
            {
                if(child.gameObject.activeSelf == true)
                {
                    activeChildCnt++;
                }
            }

            if(voiceAudioSource.isPlaying == false && activeChildCnt == 0) 
            {
                voiceAudioSource.PlayOneShot(ammunitionZeroClip);
            }
            return;
        }
        // Not available : Beep sound
        if(availableWeaponSlot == null)
        {
            weaponAudioSource.PlayOneShot(cooldownClip);
            return;
        }

        Vector3 missilePosition;
        
        // Start Cooldown
        availableWeaponSlot.StartCooldown();

        // Get from Object Pool and Launch
        GameObject missile = objectPool.GetPooledObject();
        Missile missileScript = missile.GetComponent<Missile>();
        
        // Select Launch Position
        if(missileScript.maxActivePayload == 1)
        {
            missilePosition = centerMissileTransform.position;
        }
        else
        {
            if(weaponCnt % 2 == 1)
            {
                missilePosition = rightMissileTransform.position;
            }
            else
            {
                missilePosition = leftMissileTransform.position;
            }
        }

        missile.transform.position = missilePosition;
        missile.transform.rotation = transform.rotation;
        missile.SetActive(true);

        TargetObject targetObject = (target != null && GameManager.TargetController.IsLocked == true) ? target : null;

        // Lockable: Launch(TargetObject, ...)
        // else: Launch(GuidedPosition, ...)
        if(missileScript.isLockable == true)
        {
            missileScript.Launch(targetObject, aircraftController.Speed + missileScript.additionalReleaseSpeed, gameObject.layer);
        }
        else
        {
            Vector3 forwardPosition = transform.position + transform.forward * 100;
            missileScript.Launch(forwardPosition, aircraftController.Speed + missileScript.additionalReleaseSpeed, gameObject.layer, gameObject);
        }
        
        
        weaponCnt--;

        uiController.SetMissileText(missileCnt);
        uiController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
        
        if(missileScript.launchAudio != null)
        {
            missileAudioSource.PlayOneShot(missileScript.launchAudio);
        }
        else
        {
            missileAudioSource.PlayOneShot(SoundManager.Instance.GetMissileLaunchClip());
        }
    }


    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            useSpecialWeapon = !useSpecialWeapon;
            SetUIAndTarget();

            if(specialWeaponScript != null)
            {
                specialWeaponScript.enabled = useSpecialWeapon;
            }
        }
    }

    
    void SetUIAndTarget(bool playAudio = true)
    {
        Missile switchedMissile = (useSpecialWeapon == true) ? specialWeapon : missile;
        WeaponSlot[] weaponSlots = (useSpecialWeapon == true) ? spwSlots : mslSlots;

        uiController.SetGunText(bulletCnt);
        uiController.SetMissileText(missileCnt);
        uiController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
        uiController.SwitchWeapon(weaponSlots, useSpecialWeapon, switchedMissile, playAudio);
        GameManager.TargetController.SwitchWeapon(switchedMissile);
    }

    void SetArmament()
    {
        // Guns
        fireInterval = 60.0f / gunRPM;

        // Missiles
        missileCnt = missile.payload;
        missileCooldownTime = missile.cooldown;
        for(int i = 0; i < 2; i++)
        {
            mslSlots[i] = new WeaponSlot(missileCooldownTime);
        }

        // Special Weapons
        specialWeaponCnt = specialWeapon.payload;
        spwCooldownTime = specialWeapon.cooldown;
        specialWeaponName = specialWeapon.missileName;

        for(int i = 0; i < 2; i++)
        {
            spwSlots[i] = new WeaponSlot(spwCooldownTime);
        }
    }

    void SetMinimapCamera()
    {
        // Minimap
        Vector2 distance = new Vector3(transform.position.x - target.transform.position.x, 
                                       transform.position.z - target.transform.position.z);
        minimapController.SetZoom(distance.magnitude);
    }

    void Awake()
    {
        aircraftController = GetComponent<AircraftController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        uiController = GameManager.UIController;
        
        missilePool = GameManager.Instance.missileObjectPool;
        specialWeaponPool = GameManager.Instance.specialWeaponObjectPool;
        bulletPool = GameManager.Instance.bulletObjectPool;

        missilePool.poolObject = missile.gameObject;
        specialWeaponPool.poolObject = specialWeapon.gameObject;

        useSpecialWeapon = false;

        SetArmament();
        SetUIAndTarget(false);
    }

    void Update()
    {
        // UI
        foreach(WeaponSlot slot in mslSlots)
        {
            slot.UpdateCooldown();
        }
        foreach(WeaponSlot slot in spwSlots)
        {
            slot.UpdateCooldown();
        }

        if(target != null)
        {
            SetMinimapCamera();
        }
    }
}
