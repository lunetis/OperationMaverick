using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMaverick : MissionManager
{
    enum CanyonStatus
    {
        NOT_ENTERED,
        ENTERED,
        LEAVED
    }

    public float canyonEnterPositionZ;
    public float canyonLeavePositionZ;
    public float warningAltitude;
    public float failAltitude;

    public float phase2BombingLeaveAltitude = 2640;

    bool hasSAMsActivated;

    CanyonStatus canyonStatus;

    
    [SerializeField]
    AlertUIController alertUIController;

    [SerializeField]
    RedTimer redTimer;

    [SerializeField]
    int timeLimit;

    [SerializeField]
    GameObject[] SAMs;

    List<EnemyWeaponController> SAMControllers;
    List<SAM> SAMScripts;

    [SerializeField]
    GameObject[] enemyAircrafts;

    int remainingEnemyAircraftCnt;

    [SerializeField]
    Transform firstEnemyWaypoint;

    [SerializeField]
    [Tooltip("DEBUG only")]
    int initialPhase = 1;

    protected override void Start()
    {
        phase = initialPhase;
        canyonStatus = CanyonStatus.NOT_ENTERED;

        SAMControllers = new List<EnemyWeaponController>(SAMs.Length);
        SAMScripts = new List<SAM>(SAMs.Length);

        foreach(var SAM in SAMs)
        {
            SAMControllers.Add(SAM.GetComponent<EnemyWeaponController>());
            SAMScripts.Add(SAM.GetComponent<SAM>());
        }

        SetSAMsActive(false);
        SetEnemyAircraftsActive(false);

        remainingEnemyAircraftCnt = enemyAircrafts.Length;

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

    public void CheckBombing(bool isHit)
    {
        StopTimer();

        // Fail
        if(isHit == false)
        {
            GameManager.Instance.GameOver(false);
        }
        // Success: proceed
        else
        {
            phase = 2;
        }
    }

    void SetSAMsActive(bool active)
    {
        hasSAMsActivated = active;
        foreach(var controller in SAMControllers)
        {
            controller.enabled = active;
        }
        foreach(var script in SAMScripts)
        {
            script.enabled = active;
        }
    }

    void SetEnemyAircraftsActive(bool active)
    {
        foreach(var enemy in enemyAircrafts)
        {
            enemy.SetActive(active);
            enemy.GetComponent<EnemyAircraft>().ForceChangeWaypoint(firstEnemyWaypoint.position);
        }
    }


    void CheckPhase1()
    {
        Vector3 playerPos = GameManager.PlayerAircraft.transform.position;

        if(canyonStatus == CanyonStatus.NOT_ENTERED && canyonEnterPositionZ < playerPos.z)
        {
            canyonStatus = CanyonStatus.ENTERED;
            redTimer.RemainTime_RedTimerOnly = timeLimit;
            redTimer.gameObject.SetActive(true);
        }

        if(canyonStatus == CanyonStatus.ENTERED)
        {
            // Altitude Restriction
            if(playerPos.z < canyonLeavePositionZ)
            {
                CheckAltitude(playerPos);
            }
            else
            {
                canyonStatus = CanyonStatus.LEAVED;
                alertUIController.SetCautionUI(false);
            }
        }
    }

public void DecreaseEnemyAircraftCnt()
{
    remainingEnemyAircraftCnt--;

    // Some additional voice comms can be added

    if(remainingEnemyAircraftCnt == 0)
    {
        GameManager.Instance.MissionAccomplish();
    }
}

    void CheckPhase2()
    {
        if(hasSAMsActivated == false && GameManager.PlayerAircraft.transform.position.y > phase2BombingLeaveAltitude)
        {
            SetSAMsActive(true);
            SetEnemyAircraftsActive(true);
            hasSAMsActivated = true;
        }
    }

    void StopTimer()
    {
        redTimer.enabled = false;

        Invoke("RemoveTimer", 3);
    }

    void RemoveTimer()
    {
        redTimer.gameObject.SetActive(false);
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
