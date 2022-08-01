using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyWeaponController))]
public class SAM : MonoBehaviour
{
    Transform targetTransform;

    EnemyWeaponController enemyWeaponController;

    [SerializeField]
    bool enableByTargetAltitude = true;
    
    [SerializeField]
    Transform missileLauncherBody;
    [SerializeField]
    float rotateLerpAmount = 1.0f;

    void Awake()
    {
        enemyWeaponController = GetComponent<EnemyWeaponController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = enemyWeaponController.TargetObject?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(targetTransform == null)
        {
            targetTransform = enemyWeaponController.TargetObject?.transform;
        }

        // disable
        if(enableByTargetAltitude == true && targetTransform.position.y < transform.position.y)
        {
            enemyWeaponController.enabled = false;
        }
        else
        {
            enemyWeaponController.enabled = true;
            Vector3 lookVector = targetTransform.position - transform.position;
            float lookAngle = Mathf.Atan2(lookVector.x, lookVector.z) * Mathf.Rad2Deg;
            missileLauncherBody.rotation = Quaternion.Slerp(missileLauncherBody.rotation, Quaternion.Euler(0, lookAngle, 0), rotateLerpAmount * Time.deltaTime);
        }
    }
}
