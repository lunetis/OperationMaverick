using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    protected Transform parent;
    protected Rigidbody rb;

    [SerializeField]
    protected TargetObject target;
    protected float speed;
    public string missileName;

    public bool isLockable = true;

    [Header("Properties")]

    [SerializeField]
    protected float damage;

    public bool isSpecialWeapon;
    public float maxSpeed;
    public float accelAmount;
    public float turningForce;

    
    public float additionalReleaseSpeed = 15;

    [SerializeField]
    [Range(0, 1)]
    protected float smartTrackingRate = 0.3f;

    [Space(10)]
    public float boresightAngle;
    public float lifetime;

    [Space(10)]
    public float targetSearchSpeed;
    public float lockDistance;

    [Space(10)]
    public float cooldown;
    public int payload;

    public int maxActivePayload = 2;

    // UI
    [Header("UI")]
    public Sprite missileFrameSprite;
    public Sprite missileFillSprite;
    public MinimapSprite minimapSprite;
    
    // Effect
    GameObject smokeTrailEffect;
    public Transform smokeTrailPosition;

    protected bool isHit = false;
    protected bool isDisabled = false;
    protected bool hasWarned = false;

    // Prediction
    protected AudioSource audioSource;
    protected Rigidbody targetRigidbody = null;
    
    [Tooltip("If null, plays default missile launch audio clip (Can be found at SoundManager)")]
    public AudioClip launchAudio;

    public bool IsHit
    {
        set { isHit = value; }
    }

    public bool HasWarned
    {
        get { return hasWarned; }
        set { hasWarned = value; }
    }

    public bool IsDisabled
    {
        get { return isDisabled; }
    }

    public void Launch(TargetObject target, float launchSpeed, int layer)
    {
        this.target = target;
        
        targetRigidbody = target?.GetComponent<Rigidbody>();
        minimapSprite.SetMinimapSpriteVisible(target != null);
        isDisabled = (target == null);

        // Send Message to object that it is locked on
        target?.AddLockedMissile(this);

        speed = launchSpeed;
        gameObject.layer = layer;
        
        smokeTrailEffect = GameManager.Instance.smokeTrailEffectObjectPool.GetPooledObject();
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.GetComponent<SmokeTrail>()?.SetFollowTransform(smokeTrailPosition);
            smokeTrailEffect.SetActive(true);
        }
        
        Invoke("DisableMissile", lifetime);
    }

    public virtual void Launch(Vector3 guidedPosition, float launchSpeed, int layer, GameObject launcher)
    {
        
    }

    Vector3 GetPredictedTargetPosition()
    {
        if(targetRigidbody == null) return target.transform.position;

        Vector3 predictedPos = target.transform.position;
        float timeToCatchUp = MathHelper.GetTimeToCatchUp(targetRigidbody, rb);
        float angle = Vector3.Angle(transform.forward, target.transform.forward);

        if(timeToCatchUp > 0)
        {
            // Considering target velocity
            predictedPos += timeToCatchUp * targetRigidbody.velocity;
        }
        else
        {
            // Not considering target velocity
            float distance = Vector3.Distance(target.transform.position, transform.position);
            predictedPos += (distance / speed) * (Mathf.Cos(angle) + 1) * 0.5f * targetRigidbody.velocity;
        }

        return predictedPos;
    }

    protected virtual void LookAtTarget()
    {
        if(target == null)
            return;

        Vector3 targetPos = Vector3.Lerp(target.transform.position, GetPredictedTargetPosition(), smartTrackingRate);
        
        // For ground objects: give offset (0, 5, 0) if the distance between missile and the target > 100 (on unity coordinates)
        if(target.Info.IsGroundObject == true && Vector3.Distance(target.transform.position, transform.position) > 100)
        {
            targetPos.y += 5;
        }
        Vector3 targetDir = target.transform.position - transform.position;

        // Angle : Based on current target position
        float angle = Vector3.Angle(targetDir, transform.forward);

        if(angle > boresightAngle)
        {
            // UI
            ShowMissedLabel();
            minimapSprite.SetMinimapSpriteVisible(false);

            isDisabled = true;
            
            // Send Message to object that it is no more locked on
            target.RemoveLockedMissile(this);
            target = null;
            
            return;
        }

        // LookRotation: Based on target's predicted position
        Quaternion lookRotation = Quaternion.LookRotation(targetPos - transform.position);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turningForce * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if(target != null && other.gameObject == target.gameObject)
        {
            isHit = true;
        }
        other.gameObject.GetComponent<TargetObject>()?.OnDamage(damage, gameObject.layer);

        Explode();
        DisableMissile();
    }

    protected void Explode(float explosionScale = 1)
    {
        // Instantiate in world space
        GameObject effect = GameManager.Instance.explosionEffectObjectPool.GetPooledObject();
        effect.transform.position = transform.position;
        effect.transform.rotation = transform.rotation;
        effect.transform.localScale *= explosionScale;
        effect.SetActive(true);
    }


    // Called by ECM System
    public void EvadeByECM(Vector3 randomPosition)
    {
        if(target != null)
        {
            target.RemoveLockedMissile(this);

            if(isDisabled == false && isHit == false)
            {
                ShowMissedLabel();
            }
        }

        target = null;

        Vector3 randomDirection = randomPosition - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(randomDirection);
        rb.rotation = lookRotation;
    }


    public void RemoveTarget()
    {
        target = null;
        DisableMissile();
    }

    protected virtual void DisableMissile()
    {
        
        gameObject.SetActive(false);
    }

    protected void ShowMissedLabel()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Missed);
        }
    }

    protected virtual void AdjustValuesByDifficulty()
    {
        
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        parent = transform.parent;
    }

    void Start()
    {
        AdjustValuesByDifficulty();
    }
    

    void OnDisable()
    {
        hasWarned = false;
        
        // Send Message to object that it is no more locked on
        if(target != null)
        {
            target.RemoveLockedMissile(this);

            if(isDisabled == false && isHit == false)
            {
                ShowMissedLabel();
            }
        }
        
        isDisabled = true;
        transform.parent = parent;
        
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.GetComponent<SmokeTrail>().StopFollow();
        }
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        CancelInvoke();
    }

    void FixedUpdate()
    {
        LookAtTarget();
        if(speed < maxSpeed)
        {
            speed += accelAmount * Time.fixedDeltaTime;
        }

        rb.velocity = transform.forward * speed;
    }
}
