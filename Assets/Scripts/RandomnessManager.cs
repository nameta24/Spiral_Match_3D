using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RandomnessManager : MonoBehaviour
{
    public static RandomnessManager instance;
    [SerializeField] public GameObject[] levels;

    private void Awake()
    {
        instance = this;
        foreach (GameObject level in levels)
        {
            level.SetActive(false);
        }
        levels[Mathf.Clamp(PlayerPrefs.GetInt("CurrentLevel", 1)-1,0,5)].SetActive(true);
    }

    public int GenerateRandomMovesCount()
    {
        int moves = 35;
        moves += Random.Range(-8, 8);
        PlayerPrefs.SetInt("RandomMovesCount", moves);
        PlayerPrefs.SetInt("CanSpawnRandomLevel", 0);

        return   PlayerPrefs.GetInt("RandomMovesCount",moves);
    }

    public GridGenerator3D.TargetItem[] ReturnShuffledArray(GridGenerator3D.TargetItem[] items)
    {
        // Shuffle the array
        for (int i = items.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap elements
            GridGenerator3D.TargetItem temp = items[i];
            items[i] = items[randomIndex];
            items[randomIndex] = temp;
        }

      
        // Determine new size of the array
        int newSize = items.Length;
        if (Random.Range(0,3)== 1) // 50% chance to reduce size by one
        {
            newSize--;
        }

        return items.Take(newSize).ToArray();
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel", 1)+1);
            PlayerPrefs.SetInt("CanSpawnRandomLevel", 1);
            SceneManager.LoadScene(0);
        }
    }
}