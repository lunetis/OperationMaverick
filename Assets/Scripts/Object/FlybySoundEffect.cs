using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlybySoundEffect : MonoBehaviour
{
    public float playDistance = 200;
    bool isPlaying;

    [SerializeField]
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = false;  
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Check Distance
        float distance = Vector3.Distance(transform.position, GameManager.PlayerAircraft.transform.position);

        if(distance < playDistance && isPlaying == false)
        {
            isPlaying = true;
            audioSource.PlayOneShot(SoundManager.Instance.GetFlybyClip());
        }

        if(isPlaying == true && distance < playDistance)
        {
            isPlaying = false;
        }
    }
}
