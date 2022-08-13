using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionAudio : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip customAudioClip;

    void PlayExplosionClip()
    {
        AudioClip clip = (audioSource.clip != null) ? audioSource.clip : SoundManager.Instance.GetExplosionClip();
        audioSource.PlayOneShot(clip);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        float distance = GameManager.Instance.GetDistanceFromPlayer(transform);
        Invoke("PlayExplosionClip", distance * SoundManager.Instance.DistanceMultiplier);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
