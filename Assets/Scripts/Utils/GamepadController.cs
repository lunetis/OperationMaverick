using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadController : MonoBehaviour
{
    Gamepad gamepad;

    public bool isGunFiring;

    [Range(0, 1)]
    public float gunVibrateAmount;

    float damageVibrateDuration;

    public float DamageVibrateDuration
    {
        set
        {
            damageVibrateDuration = value;
            damageVibrateDurationReciprocal = 1 / value;
        }
    }

    float damageVibrateDurationReciprocal;

    float damageVibrateAmount;

    public void VibrateByDamage()
    {
        damageVibrateAmount = 1;
    }

    public void DisableVibrate()
    {
        damageVibrateAmount = 0;
        isGunFiring = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        gamepad = Gamepad.current;

        if(gamepad == null)
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float vibrateAmount = damageVibrateAmount;
        
        // If firing guns, gun vibration must go on
        if(isGunFiring == true)
        {
            vibrateAmount = Mathf.Max(damageVibrateAmount, gunVibrateAmount);
        }
        // If damage vibration is not ended
        if(damageVibrateAmount > 0)
        {
            damageVibrateAmount -= Time.deltaTime * damageVibrateDurationReciprocal;
        }
        else
        {
            damageVibrateAmount = 0;
        }

        gamepad.SetMotorSpeeds(vibrateAmount, vibrateAmount);
    }

    void OnDisable()
    {
        if(gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }
}
