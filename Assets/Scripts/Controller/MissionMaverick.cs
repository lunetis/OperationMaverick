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

    [SerializeField]
    List<TargetObject> targetsEnableOnPhase2;

    int remainingEnemyAircraftCnt;

    [SerializeField]
    Transform firstEnemyWaypoint;



    [Space(20)]
    [Header("Transcripts")]
    
    [SerializeField]
    string scriptOnHighAltitude;

    bool hasWarned;
    
    [SerializeField]
    List<string> scriptsOnAltitudeFail;
    
    [SerializeField]
    List<string> scriptsOnLeaveCanyon;

    [SerializeField]
    [Tooltip("This scripts must contain 3 scripts. 1st and 2nd will be printed randomly, and 3rd will be printed after that.")]
    List<string> scriptsOnMiss;

    [SerializeField]
    List<string> scriptsOnHit;
    
    [SerializeField]
    List<string> scriptsOnPhase2;

    
    [SerializeField]
    List<string> scriptsForRemainEnemies;
    
    [SerializeField]
    string scriptForOneTarget;

    [SerializeField]
    List<string> scriptsOnMissionAccomplish;

    
    [SerializeField]
    List<string> scriptsOnCanyon;

    [SerializeField]
    TipUIController tipUIController;

    public Transform phase1start;
    public Transform phase2start;

    

    [SerializeField]
    [Header("DEBUG only")]
    int initialPhase = 1;
    void CheckAltitude(Vector3 playerPos)
    {
        if(GameManager.Instance.IsGameOver == true)
            return;
        
        if(playerPos.y > warningAltitude)
        {
            if(playerPos.y < failAltitude)
            {
                // Caution
                if(hasWarned == false)
                {
                    GameManager.ScriptManager.AddScriptAtFront(scriptOnHighAltitude);
                    hasWarned = true;
                }

                // Do not print again
                alertUIController.SetCautionUI(true, true);
            }
            else
            {
                // Fail
                GameOver();
                AddScript(scriptsOnAltitudeFail);
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
            // This scripts must contain 3 scripts. 
            // 1st and 2nd will be printed randomly, and 3rd will be printed after that.
            GameManager.ScriptManager.ClearScriptQueue();
            AddScriptRandomly(scriptsOnMiss.GetRange(0, 2));
            AddScript(scriptsOnMiss[2]);
        }
        // Success: proceed
        else
        {
            AddScriptRandomly(scriptsOnHit);
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
        foreach(var target in targetsEnableOnPhase2)
        {
            target.enabled = active;
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


    // Just calls ScriptManager functions
    void AddScript(string scriptKey)
    {
        GameManager.ScriptManager.AddScript(scriptKey);
    }

    void AddScript(List<string> scriptKeyList)
    {
        GameManager.ScriptManager.AddScript(scriptKeyList);
    }

    void AddScriptRandomly(List<string> scriptKey)
    {
        GameManager.ScriptManager.AddScriptRandomly(scriptKey);
    }


    void CheckPhase1()
    {
        Vector3 playerPos = GameManager.PlayerAircraft.transform.position;

        // When entering the canyon
        if(canyonStatus == CanyonStatus.NOT_ENTERED && canyonEnterPositionZ < playerPos.z)
        {
            canyonStatus = CanyonStatus.ENTERED;
            redTimer.RemainTime_RedTimerOnly = timeLimit;
            redTimer.gameObject.SetActive(true);

            Invoke("PlayScriptsOnCanyon", Random.Range(20, 100));
        }

        // In the canyon
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
                
                AddScript(scriptsOnLeaveCanyon);
            }
        }
    }

    public void DecreaseEnemyAircraftCnt()
    {
        remainingEnemyAircraftCnt--;

        if(GameManager.Instance.IsGameOver == true)
            return;

        // 1st plane down, 2nd plane down, ...
        AddScript(scriptsForRemainEnemies[enemyAircrafts.Length - remainingEnemyAircraftCnt - 1]);

        if(remainingEnemyAircraftCnt == 1)
        {
            AddScript(scriptForOneTarget);
        }

        if(remainingEnemyAircraftCnt == 0)
        {
            // Mission Accomplish
            ResultData.score = GameManager.PlayerAircraft.Score;
            GameManager.Instance.MissionAccomplish();
            AddScriptRandomly(scriptsOnMissionAccomplish);
        }
    }

    void CheckPhase2()
    {
        if(hasSAMsActivated == false && GameManager.PlayerAircraft.transform.position.y > phase2BombingLeaveAltitude)
        {
            AddScript(scriptsOnPhase2);
            SetSAMsActive(true);
            SetEnemyAircraftsActive(true);
            hasSAMsActivated = true;
        }
    }

    // Misc. scripts (Can be invoked)
    void StopTimer()
    {
        redTimer.enabled = false;

        Invoke("RemoveTimer", 3);
    }

    void RemoveTimer()
    {
        redTimer.gameObject.SetActive(false);
    }

    void PlayScriptsOnCanyon()
    {
        if(GameManager.Instance.IsGameOver == true)
            return;

        AddScript(scriptsOnCanyon);
    }

    void UpdateMission()
    {
        GameManager.UIController.SetLabel(AlertUIController.LabelEnum.MissionUpdated);
    }
    
    void GameOver()
    {
        GameManager.Instance.GameOver(false, false, false);
    }

    void FadeOut()
    {
        if(GameManager.Instance.IsGameOver == true)
        {
            GameManager.Instance.GameOverFadeOut();
        }
        else
        {
            GameManager.Instance.MissionAccomplishedFadeOut();
        }
    }

    void ShowTip1()
    {
        tipUIController.ShowTip("TIP_1");
    }

    void ShowTip2()
    {
        tipUIController.ShowTip("TIP_2");
    }

    // ================================

    public override void OnGameOver(bool isDead)
    {
        if(isDead == true)
        {
            AddScriptRandomly(onDeadScripts);
        }
        else
        {
            AddScriptRandomly(onMissionFailedScripts);
        }
    }

    public override void SetupForRestartFromCheckpoint()
    {
        if(phase == 1)
        {
            ResultData.elapsedTime = 0;
        }
    }
    
    private void Awake()
    {
        SAMControllers = new List<EnemyWeaponController>(SAMs.Length);
        SAMScripts = new List<SAM>(SAMs.Length);

        foreach(var SAM in SAMs)
        {
            SAMControllers.Add(SAM.GetComponent<EnemyWeaponController>());
            SAMScripts.Add(SAM.GetComponent<SAM>());
        }
        SetSAMsActive(false);
        SetEnemyAircraftsActive(false);
    }

    protected override void Start()
    {
        hasWarned = false;

        switch(GameSettings.difficultySetting)
        {
            case GameSettings.Difficulty.EASY:      timeLimit = 270; break;
            case GameSettings.Difficulty.NORMAL:    timeLimit = 210; break;
            case GameSettings.Difficulty.HARD:      timeLimit = 150; break;
            case GameSettings.Difficulty.ACE:       timeLimit = 135; break;
            default:                                timeLimit = 210; break;
        }

        if(initialPhase > 1)
        {
            phase = initialPhase;
        }
        
        canyonStatus = CanyonStatus.NOT_ENTERED;

        remainingEnemyAircraftCnt = enemyAircrafts.Length;

        SetResultData();
        
        GameManager.UIController.SetRemainTime(missionInfo.TimeLimit);

        if(phase == 1)
        {
            GameManager.PlayerAircraft.transform.SetPositionAndRotation(phase1start.position, phase1start.rotation);
            GameManager.ScriptManager.AddScript(onMissionStartScripts);
        }
        else if(phase == 2)
        {
            GameManager.ScriptManager.ClearScriptQueue();
            GameManager.PlayerAircraft.transform.SetPositionAndRotation(phase2start.position, phase2start.rotation);
            // You can't use special weapon at phase 2
            GameManager.WeaponController.SpecialWeaponCnt = 0;
        }
    }


    void Update()
    {
        if(GameManager.Instance.IsGameOver == true)
            return;
            
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
