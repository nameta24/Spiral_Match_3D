using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class TSSceneLoader : MonoBehaviour
{
    private void Start()
    {
        TinySauce.SubscribeOnInitFinishedEvent(LoadScene);
    }

    private void LoadScene(bool adConsent, bool trackingConsent)
    {
        SceneManager.LoadScene(1);
    }
}
