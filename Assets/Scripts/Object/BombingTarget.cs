using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombingTarget : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        LaserGuidedBomb lgb = other.GetComponent<LaserGuidedBomb>();

        if(lgb != null)
        {
            lgb.IsHit = true;
        }
    }
}
