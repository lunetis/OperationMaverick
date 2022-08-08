using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CautionAlert : MonoBehaviour
{
    public bool isCautionEnabled;
    bool isPlayingAudio;

    [SerializeField]
    GameObject cautionUI;
    
    [SerializeField]
    AudioSource cautionAudio;

    [SerializeField]
    float cautionAudioRepeatDelay = 1.8f;

    // Start is called before the first frame update
    void Start()
    {
        isCautionEnabled = false;
        isPlayingAudio = false;
    }

    void PlayCautionAudio()
    {
        cautionAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        cautionUI.SetActive(isCautionEnabled);

        if(isCautionEnabled == true)
        {
            if(isPlayingAudio == false)
            {
                isPlayingAudio = true;
                InvokeRepeating("PlayCautionAudio", 0, cautionAudioRepeatDelay);
            }
        }
        else
        {
            isPlayingAudio = false;
            CancelInvoke();
        }
    }
}
