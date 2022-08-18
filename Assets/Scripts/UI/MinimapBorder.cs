using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBorder : MonoBehaviour
{
    [SerializeField]
    float width;
    [SerializeField]
    float height;

    [SerializeField]
    float borderThickness = 10;

    [SerializeField]
    float warningDistance = 500;

    SpriteRenderer spriteRenderer;

    [SerializeField]
    AlertUIController alertUIController;

    bool isCautionEnabled;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = Vector3.zero;
        isCautionEnabled = false;
    }

    public void SetMinimapBorderSize(float cameraViewWidth)
    {
        // 100000f: Just a base value
        float size =  100000f / cameraViewWidth / borderThickness;
        float scale = width / size;
        transform.localScale = new Vector3(scale, scale, scale);
        spriteRenderer.size = new Vector3(width, height) / scale;
    }

    void CheckPlayerPosition()
    {
        Vector3 pos = GameManager.PlayerAircraft.transform.position;
        Vector3 center = transform.position;
        float upperLimit = center.z + height * 0.5f;
        float lowerLimit = center.z - height * 0.5f;
        float leftLimit = center.x - width * 0.5f;
        float rightLimit = center.x + width * 0.5f;
        
        // Warning
        if(pos.x > upperLimit - warningDistance || pos.x < lowerLimit + warningDistance || 
           pos.z < leftLimit + warningDistance || pos.z > rightLimit - warningDistance)
        {
            // Fail
            if(pos.x > upperLimit || pos.x < lowerLimit || pos.z < leftLimit || pos.z > rightLimit)
            {
                GameManager.Instance.GameOver(false, false, true);
                SetCautionUI(false);
            }
            else
            {
                SetCautionUI(true);
            }
        }
        else
        {
            SetCautionUI(false);
        }
    }

    void SetCautionUI(bool enabled)
    {
        if(isCautionEnabled == enabled)
            return;

        isCautionEnabled = enabled;
        alertUIController.SetCautionUI(enabled);
    }

    void Update()
    {
        CheckPlayerPosition();
    }
}
