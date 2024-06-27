using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameplayUiManager : MonoBehaviour
{
    public static GameplayUiManager instance;
  [BoxGroup("Booster Fill")]  [SerializeField] private Slider slider;
  [BoxGroup("Booster Fill")]  [SerializeField] public int selectionsToReachBooster;


 [Space] [BoxGroup("3 Star System")] 
  public GameObject[] stars,particlesVfx;

  [Space] [BoxGroup("HUD")] 
  public TextMeshProUGUI currentLevelTxt;

  [HideInInspector]  public int currentSelections;
        
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        currentLevelTxt.text="Level " + PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    // Update is called once per frame
    void Update()
    {
        slider.DOValue(currentSelections, .4f);
    }

    public void Show3Stars()
    {
        StartCoroutine(Start3StarSequence());
    }

    IEnumerator Start3StarSequence()
    {
        yield return new WaitForSeconds(.2f);
        if (((float)LinkerStatic.instance.remainingMoves / (float)GridGenerator3D.instance.maxMovesCount) * 100 >= 0)
        {
            //1 star
            stars[0].SetActive(true);
            particlesVfx[0].SetActive(true);

            AudioManager.Instance.PlaySound(AudioManager.Instance.starUi);
            MyHapticsManager.Instance.PlayMediumHaptics();


        }
        yield return new WaitForSeconds(.3f);
        if (((float)LinkerStatic.instance.remainingMoves / (float)GridGenerator3D.instance.maxMovesCount) * 100 > 15)
        {
            //2 stars
            stars[1].SetActive(true);
            particlesVfx[1].SetActive(true);
            MyHapticsManager.Instance.PlayMediumHaptics();

            AudioManager.Instance.PlaySound(AudioManager.Instance.starUi);

        }
        yield return new WaitForSeconds(.3f);

        if (((float)LinkerStatic.instance.remainingMoves / (float)GridGenerator3D.instance.maxMovesCount) * 100 > 25)
        {
            //3 stars
            stars[2].SetActive(true);
            particlesVfx[2].SetActive(true);
            MyHapticsManager.Instance.PlayMediumHaptics();

            AudioManager.Instance.PlaySound(AudioManager.Instance.starUi);

        }
    }

   
    
}
