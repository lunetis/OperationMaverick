using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    [SerializeField]
    List<AudioClip> missileLaunchClips;

    [SerializeField]
    List<AudioClip> explosionClips;
    
    [SerializeField]
    List<AudioClip> gunHitClips;

    [SerializeField]
    List<AudioClip> flybyClips;

    [SerializeField]
    float distanceMultiplier = 0.001f;

    public float DistanceMultiplier
    {
        get { return distanceMultiplier; }
    }

    AudioClip GetClipRandomly(List<AudioClip> clips)
    {
        return clips[Random.Range(0, clips.Count)];
    }

    public AudioClip GetMissileLaunchClip()
    {
        return GetClipRandomly(missileLaunchClips);
    }
    
    public AudioClip GetExplosionClip()
    {
       return GetClipRandomly(explosionClips);
    }

    public AudioClip GetGunHitClip()
    {
        return GetClipRandomly(gunHitClips);
    }
    
    public AudioClip GetFlybyClip()
    {
        return GetClipRandomly(flybyClips);
    }


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
