using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlareController : MonoBehaviour
{
    [SerializeField]
    int flareCnt;

    [SerializeField]
    GameObject flareEffectPrefab;

    [SerializeField]
    int effectCnt = 5;

    [SerializeField]
    float effectCreateDelay = 0.3f;

    [SerializeField]
    Rigidbody aircraftRigidbody;
    
    public void UseFlare(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            if(flareCnt == 0)
                return;

            GameManager.Instance.DisableAllMissiles();
            GameManager.UIController.SetFlareText(--flareCnt);

            StartCoroutine(CreateFlareEffect());
        }
    }

    IEnumerator CreateFlareEffect()
    {
        for(int i = 0; i < effectCnt; i++)
        {
            GameObject effect = Instantiate(flareEffectPrefab, transform.position, transform.rotation);
            Rigidbody effectRb = effect.GetComponent<Rigidbody>();
            effectRb.velocity = (aircraftRigidbody.velocity * 0.8f) + 
                                new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f) - 15, Random.Range(-2f, 2f));

            yield return new WaitForSeconds(effectCreateDelay);
        }
    }

    void Start()
    {
        GameManager.UIController.SetFlareText(flareCnt);
    }
}
