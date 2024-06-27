using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using static GridGenerator3D;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //GameObjects
    public GameObject winPanel;
    public GameObject failPanel,targetReachVfx,winConfetti;

   
    //Prefab for UI
    public GameObject CombinedTargetUIPrefab;

    //Transforms
    public Transform targetItemsParent; // Parent transform for instantiated UI Texts

    //Lists
    private List<TMP_Text> targetItemTexts = new List<TMP_Text>();

   [HideInInspector] public bool hasWon, hasFailed;

   public static LevelManager instance;
   private void Awake()
   {
       instance = this;
   }

   void Start()
   {

   }
    /// <summary>
    /// initialses interface
    /// </summary>
  public  void InitializeUI()
    {
        for (int i = 0; i < GridGenerator3D.instance.targetItems.Length; i++)
        {
            var targetItem = GridGenerator3D.instance.targetItems[i];

            GameObject newCombinedUI = Instantiate(CombinedTargetUIPrefab, targetItemsParent);
            TargetItemUi targetItemUi = newCombinedUI.GetComponent<TargetItemUi>();
            if (!targetItem.isRainBowTile)
            {
                targetItemUi.ID = targetItem.prefab.GetComponent<Cell>().ID;
            }
            Image imageComponent = newCombinedUI.GetComponentInChildren<Image>();
            TMP_Text textComponent = newCombinedUI.GetComponentInChildren<TMP_Text>();

            // Set the image and text values
            imageComponent.sprite = targetItem.itemImage;
            if (!targetItem.isRainBowTile)
            {
                textComponent.text = $"{targetItem.prefab.name}: {targetItem.targetCount}";
            }
            // textComponent.color = targetItem.prefab.GetComponent<Renderer>().material.color;
            targetItemTexts.Add(textComponent);
            textComponent.text = targetItem.targetCount.ToString();
        }
        CombinedTargetUIPrefab.SetActive(false);
    }
    /// <summary>
    /// Update the UI with the new target item counts
    /// </summary>
    /// <param name="updatedTargetItems"></param>
    public void UpdateTargetItemUI(List<TargetItem> updatedTargetItems)
    {
        // Create a dictionary to map IDs to TMP_Text components
        var textLookup = targetItemTexts.ToDictionary(
            text => text.GetComponentInParent<TargetItemUi>().ID, 
            text => text);

        foreach (var updatedItem in updatedTargetItems)
        {
            int updatedItemID = updatedItem.prefab.GetComponent<Cell>().ID;

            // Check if the dictionary contains the updated item ID
            if (textLookup.TryGetValue(updatedItemID, out TMP_Text correspondingText))
            {
                // Update the text if the ID is found
                string tempCount = correspondingText.text;
                correspondingText.text = updatedItem.targetCount.ToString();

                if (correspondingText.text != tempCount)
                {
                    correspondingText.transform.parent.GetComponent<DOTweenAnimation>().DORestartById("Shake");
                    AudioManager.Instance.PlaySound(AudioManager.Instance.spiralReachTarget);
                }

                if (correspondingText.text == "0")
                {
                    correspondingText.GetComponentInParent<TargetItemUi>().OnCompleting();
                }
            }
            else
            {
                // Handle cases where no corresponding text is found for an ID
                Debug.LogWarning("No corresponding UI text found for ID: " + updatedItemID);
            }
        }
    }

    /// <summary>
    /// on level complete method
    /// </summary>
    public void LevelComplete()
    {
        if (!hasWon)
        {
            hasWon = true;
            StartCoroutine(StartWinSequence());
        }
    }

    IEnumerator StartWinSequence()
    {
        yield return new WaitForSeconds(.75f);
        winConfetti.SetActive(true);
        TinySauce.OnGameFinished(true,0,PlayerPrefs.GetInt("CurrentLevel", 1));
        PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel", 1)+1);
        PlayerPrefs.SetInt("CanSpawnRandomLevel", 1);
        MyHapticsManager.Instance.PlayMediumHaptics();

        AudioManager.Instance.PlaySound(AudioManager.Instance.win);
        Destroy(winConfetti,4);
        yield return new WaitForSeconds(2.5f);
        MyHapticsManager.Instance.PlayMediumHaptics();

        GameplayUiManager.instance.Show3Stars();
            
        winPanel.gameObject.SetActive(true);
    }
    /// <summary>
    /// on level fail method
    /// </summary>
    public void LevelFail()
    {
        if (!hasFailed && !hasWon)
        {
            hasFailed = true;
            StartCoroutine(StartFailSequence());
        }
    }
    IEnumerator StartFailSequence()
    {
        PlayerPrefs.SetInt("CanSpawnRandomLevel", 0);
        TinySauce.OnGameFinished(false
            ,0,PlayerPrefs.GetInt("CurrentLevel", 1));

        yield return new WaitForSeconds(2);
        AudioManager.Instance.PlaySound(AudioManager.Instance.fail);
        
        yield return new WaitForSeconds(1);
        MyHapticsManager.Instance.PlayMediumHaptics();
        failPanel.gameObject.SetActive(true);
    }
    /// <summary>
    /// reloads scene
    /// </summary>
   public void ReloadScene()
    {
        MyHapticsManager.Instance.PlayMediumHaptics();
        AudioManager.Instance.PlaySound(AudioManager.Instance.uiButton);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex); // Reload the current scene
    }

    public Transform ReturnTransformToMoveSpiralTo(GameObject tileObject)
    {
        Transform transformToReturn = targetItemsParent;
        for (int i = 0; i < targetItemTexts.Count; i++)
        {
            if (targetItemTexts[i].transform.parent.GetComponent<TargetItemUi>().ID == tileObject.GetComponent<Cell>().ID)
            {
                transformToReturn = targetItemTexts[i].transform.parent;
            }
        }

        return transformToReturn;

    }
}
