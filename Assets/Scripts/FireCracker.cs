using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class FireCracker : MonoBehaviour
{
    [HideInInspector] public bool isActive;
    [SerializeField] private GameObject trail;
    private SpiralSpawner spiralSpawner;

    private void Awake()
    {
        spiralSpawner = GetComponent<SpiralSpawner>();
    }

    public void Activated()
    {
        isActive = true;
        AudioManager.Instance.PlaySound(AudioManager.Instance.fireCrackerActive);
        MyHapticsManager.Instance.PlayMediumHaptics();
        trail.SetActive(true);
    }

    public void AnimationEnded()
    {
        Destroy(transform.parent.gameObject);
        GridGenerator3D.instance.MoveDownAndReSpawn();  
    }

    private void OnEnable()
    {
       MoveNow();
    }
   

     void MoveNow()
    {
        GetComponent<DOTweenAnimation>().DOPlayById("Move");
        Destroy(gameObject,5);
    }

     private void OnTriggerEnter(Collider col)
     {
         if (col.gameObject.GetComponent<Cell>() && isActive)
         {
             col.gameObject.GetComponent<Cell>().ShowSelectionParticles();
             spiralSpawner.SpawnSpiral(col.gameObject.GetComponent<Cell>()._meshRenderer.material.color,60,
                 col.transform.position- new Vector3(0,0,GridGenerator3D.instance.depth),col.gameObject);
          MyHapticsManager.Instance.PlayMediumHaptics();
             Destroy(col.gameObject);
         }
     }
}
