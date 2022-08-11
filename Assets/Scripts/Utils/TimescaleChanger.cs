using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleChanger : MonoBehaviour
{
    [Range(0, 20)]
    public float timescale = 1;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timescale;
    }
}
