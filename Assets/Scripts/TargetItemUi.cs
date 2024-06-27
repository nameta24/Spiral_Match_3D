using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetItemUi : MonoBehaviour
{
    public int ID;
    public GameObject txt, completeICon, confetti;

    private bool hasCompleted;
    public void OnCompleting()
    {
        if (!hasCompleted)
        {
            hasCompleted = true;
            txt.SetActive(false);
            completeICon.SetActive(true);
            confetti.SetActive(true);
            //Destroy(confetti,2);
            Invoke("DeactivateEffect", 2f);
        }
       
    }
    void DeactivateEffect()
    {
        confetti.gameObject.SetActive(false);
    }
}
