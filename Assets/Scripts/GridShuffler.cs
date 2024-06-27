using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridShuffler : MonoBehaviour
{
    private GridGenerator3D gridGenerator;

    void Awake()
    {
        gridGenerator = GetComponent<GridGenerator3D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleTiles();
        }
    }

  
    public void ShuffleTiles()
    {
        for (int x = 0; x < gridGenerator.columns; x++)
        {
            for (int y = 0; y < gridGenerator.rows; y++)
            {
                for (int z = 0; z < gridGenerator.depth; z++)
                {
                    GameObject currentTile = gridGenerator.gridItems[x, y, z];

                    // Random new position
                    int randomX, randomY, randomZ;
                    do
                    {
                        randomX = Random.Range(0, gridGenerator.columns);
                        randomY = Random.Range(0, gridGenerator.rows);
                        randomZ = Random.Range(0, gridGenerator.depth);
                    } while (randomX == x && randomY == y && randomZ == z); // Ensure it's a different position

                    // Swap tiles in the array
                    GameObject temp = gridGenerator.gridItems[randomX, randomY, randomZ];
                    gridGenerator.gridItems[randomX, randomY, randomZ] = currentTile;
                    gridGenerator.gridItems[x, y, z] = temp;

                    // Update positions in the game scene
                    if (currentTile != null)
                    {
                        Vector3 newPos = new Vector3(randomX * gridGenerator.spacing, randomY * gridGenerator.spacing, randomZ * gridGenerator.spacing);
                        currentTile.transform.position = newPos;
                        currentTile.transform.DOScale(Vector3.one, .5f).From(Vector3.zero).SetEase(Ease.OutBack);

                    }

                    if (temp != null)
                    {
                        Vector3 originalPos = new Vector3(x * gridGenerator.spacing, y * gridGenerator.spacing, z * gridGenerator.spacing);
                        temp.transform.position = originalPos;
                        temp.transform.DOScale(Vector3.one, .5f).From(Vector3.zero).SetEase(Ease.OutBack);

                    }
                }
            }
        }
        AudioManager.Instance.PlaySound(AudioManager.Instance.tileShuffle);

    }

    public bool CheckForMatches()
    {
        for (int x = 0; x < gridGenerator.columns; x++)
        {
            for (int y = 0; y < gridGenerator.rows; y++)
            {
                for (int z = 0; z < gridGenerator.depth; z++)
                {
                    if (gridGenerator.gridItems[x, y, z] != null)
                    {
                        int currentID = gridGenerator.GetPrefabID(gridGenerator.gridItems[x, y, z]);

                        // Check horizontally
                        if (x <= gridGenerator.columns - 3 &&
                            gridGenerator.GetPrefabID(gridGenerator.gridItems[x + 1, y, z]) == currentID &&
                            gridGenerator.GetPrefabID(gridGenerator.gridItems[x + 2, y, z]) == currentID)
                        {
                            return true;
                        }

                        // Check vertically
                        if (y <= gridGenerator.rows - 3 &&
                            gridGenerator.GetPrefabID(gridGenerator.gridItems[x, y + 1, z]) == currentID &&
                            gridGenerator.GetPrefabID(gridGenerator.gridItems[x, y + 2, z]) == currentID)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}