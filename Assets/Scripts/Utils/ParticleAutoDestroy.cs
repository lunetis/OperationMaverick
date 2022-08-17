using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    public float duration;
    public bool destroyInsteadDisable = false;

    void OnEnable()
    {
        float distance = GameManager.Instance.GetDistanceFromPlayer(transform);
        // When the duration is undefined, get ParticleSystem's duration
        if(duration == 0)
        {
            duration = GetComponent<ParticleSystem>().main.duration;
        }
        Invoke("Disable", duration + distance * SoundManager.Instance.DistanceMultiplier);
    }

    void Disable()
    {
        if(destroyInsteadDisable == true)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }        
    }

    private void OnDisable() {
        
        CancelInvoke();
    }
}
