using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGuidanceController : MonoBehaviour
{
    [HideInInspector]
    public LaserGuidedBomb lgb;
    RaycastHit hit;
    int layerMask;
    public float raycastDistance = 1000;

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Ground");
        layerMask += 1 << LayerMask.NameToLayer("Object");
    }

    // Update is called once per frame
    void Update()
    {
        if(lgb == null)
            return;
        
        Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, layerMask);

        if(hit.collider == null)
        {
            lgb.guidedPosition = transform.position + transform.forward * raycastDistance;
        }
        else
        {
            lgb.guidedPosition = hit.point;
        }
        
    }
}
