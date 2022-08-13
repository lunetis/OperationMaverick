using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetObject))]
public class AITranscript : MonoBehaviour
{
    public float activateDistance;

    [Tooltip("Unit: Seconds, the random delay will be (delay * 0.5 - delay).")]
    public int maxScriptDelay = 60;
    public List<string> randomScriptList;

    [Range(0.2f, 1)]
    public float scriptOnFirePlayProb = 0.2f; 
    public List<string> scriptOnFire;

    public List<string> scriptOnDestroy;

    bool hasInvoked = false;


    // Start is called before the first frame update
    void Start()
    {
        if(activateDistance == 0)
        {
            hasInvoked = true;
            InvokePlayScriptRandomly();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activateDistance == 0 || hasInvoked == true)
            return;

        if(Vector3.Distance(GameManager.PlayerAircraft.transform.position, transform.position) < activateDistance)
        {
            hasInvoked = true;
            InvokePlayScriptRandomly();
        }
    }

    public void PlayScriptOnDestroy()
    {
        if(GameManager.Instance.IsGameOver == true)
            return;
            
        CancelInvoke();
        GameManager.ScriptManager.AddScript(scriptOnDestroy);
    }

    public void PlayScriptOnFire()
    {
        if(scriptOnFire == null || scriptOnFire.Count == 0)
            return;

        if(GameManager.ScriptManager.IsPrintingScript == true)
            return;
        
        if(GameManager.Instance.IsGameOver == true)
            return;

        float value = Random.Range(0f, 1f);
        if(value < scriptOnFirePlayProb)
        {
            GameManager.ScriptManager.AddScriptRandomly(scriptOnFire);
        }
    }

    void InvokePlayScriptRandomly()
    {
        float delay = Random.Range(maxScriptDelay * 0.5f, maxScriptDelay);
        Invoke("PlayScriptRandomly", delay);
    }

    void PlayScriptRandomly()
    {
        if(GameManager.Instance.IsGameOver == true)
            return;

        int index = UnityEngine.Random.Range(0, randomScriptList.Count);
        GameManager.ScriptManager.AddScript(randomScriptList[index]);
        randomScriptList.RemoveAt(index);

        if(randomScriptList.Count != 0)
        {
            InvokePlayScriptRandomly();
        }
    }
}
