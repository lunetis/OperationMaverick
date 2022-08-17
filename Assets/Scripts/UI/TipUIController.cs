using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipUIController : MonoBehaviour
{
    public float visibleTime = 5.0f;
    public TextMeshProUGUI text;

    public void ShowTip(string tipKey)
    {
        text.text = GameManager.ScriptManager.GetSubtitleText(tipKey);
        Invoke("HideTip", visibleTime);
    }

    void HideTip()
    {
        text.text = "";
    }

    void Start()
    {
        HideTip();
    }
}
