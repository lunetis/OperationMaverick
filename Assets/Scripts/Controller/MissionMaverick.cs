using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMaverick : MissionManager
{
    public float canyonEnterPositionZ;
    public float canyonLeavePositionZ;
    public float warningAltitude;
    public float failAltitude;

    
    [SerializeField]
    AlertUIController alertUIController;

    [SerializeField]
    RedTimer redTimer;

    [SerializeField]
    int timeLimit;

    bool isInCanyon;

    protected override void Start()
    {
        phase = 1;
        isInCanyon = false;
        base.Start();
    }

    void CheckAltitude(Vector3 playerPos)
    {
        if(playerPos.y > warningAltitude)
        {
            if(playerPos.y < failAltitude)
            {
                // Caution
                alertUIController.SetCautionUI(true);
            }
            else
            {
                // Fail
                GameManager.Instance.GameOver(false);
            }
        }
        else
        {
            alertUIController.SetCautionUI(false);
        }
    }


    void CheckPhase1()
    {
        Vector3 playerPos = GameManager.PlayerAircraft.transform.position;

        if(isInCanyon == false && canyonEnterPositionZ < playerPos.z)
        {
            isInCanyon = true;
            redTimer.RemainTime_RedTimerOnly = timeLimit;
            redTimer.gameObject.SetActive(true);
        }

        if(isInCanyon == true)
        {
            // Altitude Restriction
            if(playerPos.z < canyonLeavePositionZ)
            {
                CheckAltitude(playerPos);
            }
            else
            {
                isInCanyon = false;
                phase = 2;
            }
        }
    }

    void CheckPhase2()
    {
        redTimer.enabled = false;
    }


    void Update()
    {
        switch(phase)
        {
            case 1:
                CheckPhase1();
                break;

            case 2:
                CheckPhase2();
                break;
        }
        
    }
}
