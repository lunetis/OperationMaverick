using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTrigger : MonoBehaviour
{
    public List<string> subtitleKeyList;
    bool hasPrinted = false;

    void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if(mesh == null)
            return;

        mesh.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hasPrinted == true || 
           other.gameObject.layer != LayerMask.NameToLayer("Player") || 
           subtitleKeyList == null || subtitleKeyList.Count == 0)
            return;

        if(GameManager.Instance.IsGameOver == true)
            return;
        
        GameManager.ScriptManager.AddScript(subtitleKeyList);
        
        // Once the script has been printed, disable this script
        hasPrinted = true;
        this.enabled = false;
    }
}
