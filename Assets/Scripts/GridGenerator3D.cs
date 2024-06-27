using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator3D : MonoBehaviour
{
    [System.Serializable]
    public class TargetItem
    {
        public GameObject prefab;
        public Sprite itemImage;
        public int targetCount;
        public bool isRainBowTile;
    }

    public TargetItem[] targetItems; //array for target items
    public GameObject[] itemPrefabs;
    public GameObject fireCrackerTile;

    public int rows;
    public int columns;
    public int depth;
    public float spacing;
    public float respawnDelay = 0.005f;
    public int maxMovesCount; //max moves per level

    [Tooltip("1 Hard , 2 Normal , 3 Easy")]
    [Range(1,3)]  public int difficultyFactor=3;
    public int minClusterSize = 3;
    public int maxClusterSize = 11;

    [HideInInspector] public GameObject[,,] gridItems;
    private float lastRespawnTime;
    private int lastClearedPrefabID = -1; // Initialize to an invalid ID

    public static GridGenerator3D instance;

    private GridShuffler gridShuffler;
    private TileManager tileManager;

    private void Start()
    {
        int movesOriginally = maxMovesCount;

        if (PlayerPrefs.GetInt("CanSpawnRandomLevel",0)==1 && PlayerPrefs.GetInt("CurrentLevel", 1)>6)
        {
            targetItems= RandomnessManager.instance.ReturnShuffledArray(targetItems);
            maxMovesCount = RandomnessManager.instance.GenerateRandomMovesCount();
            foreach (var target in targetItems)
            {
                target.targetCount -= movesOriginally - maxMovesCount;
            }
        }
       
        LinkerStatic.maxSpiralCount = maxMovesCount; // Set the initial value
        LinkerStatic.instance.SetupMoves();

        //Spawn
        gridItems = new GameObject[columns, rows, depth];
        lastRespawnTime = Time.time;
        GenerateGridWithIdentifiableCombinations();
        LevelManager.instance.InitializeUI();
        TinySauce.OnGameStarted(PlayerPrefs.GetInt("CurrentLevel", 1));


    }

    private void Awake()
    {
        instance = this;

        gridShuffler = GetComponent<GridShuffler>();
        tileManager = GetComponent<TileManager>();
    }

   
   

    private void Update()
    {
        if (Time.time - lastRespawnTime >= respawnDelay)
        {
            // tileManager.MoveTilesDownBeforeRespawn();
            //RespawnRandomObject();
            lastRespawnTime = Time.time;
        }
    }

    public void MoveDownAndReSpawn()
    {
        StartCoroutine(delayedSequenceOfMoveAndReSpawn());
    }

    IEnumerator delayedSequenceOfMoveAndReSpawn()
    {
        yield return new WaitForSeconds(.25f);

        tileManager.MoveTilesDownBeforeRespawn();

        yield return new WaitForSeconds(.25f);
        RespawnRandomObject();
        yield return new WaitForSeconds(.5f);

        if (! gridShuffler.CheckForMatches())
        {
           gridShuffler.ShuffleTiles();
        }

        
    }

    public void RespawnRandomObject()
    {
        Vector3Int? firstEmptyPosition = null;

        // Find the first empty position from the bottom of the grid
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (gridItems[x, y, z] == null)
                    {
                        firstEmptyPosition = new Vector3Int(x, y, z);
                        break; // Break out of the inner loop
                    }
                }

                if (firstEmptyPosition != null)
                {
                    break; // Break out of the middle loop
                }
            }

            if (firstEmptyPosition != null)
            {
                break; // Break out of the outer loop
            }
        }

        // If an empty position was found and the booster can be spawned
        if (firstEmptyPosition.HasValue && CheckIfCanSpawnBoosterTile())
        {
            SpawnFireCrackerTile(firstEmptyPosition.Value);
            GameplayUiManager.instance.currentSelections = 0; // Reset the selections if needed
        }

        // Now spawn other tiles
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (gridItems[x, y, z] == null)
                    {
                        int prefabID = GetPredominantSurroundingID(x, y, z, lastClearedPrefabID);
                        GameObject prefab = GetPrefabWithID(prefabID);
                        int halfMaxClusterSize = (maxClusterSize / 2) + 1;
                        SpawnUShapeCluster(x, y, z, prefab, prefabID, Random.Range(minClusterSize, halfMaxClusterSize));
                    }
                }
            }
        }
    }
    private void SpawnFireCrackerTile(Vector3Int position)
    {
        GameObject newItem = Instantiate(fireCrackerTile,
            new Vector3(position.x * spacing, position.y * spacing, position.z * spacing), Quaternion.identity,
            transform);
        newItem.GetComponent<Cell>().ID =
            GetPrefabID(fireCrackerTile); // Assuming Cell is a component with an ID property
        gridItems[position.x, position.y, position.z] = newItem;
        AudioManager.Instance.PlaySound(AudioManager.Instance.boosterAppear);

        //  newItem.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack).From(Vector3.one / 10);
    }

    private GameObject GetPrefabWithID(int id)
    {
        foreach (GameObject prefab in itemPrefabs)
        {
            if (GetPrefabID(prefab) == id)
            {
                return prefab;
            }
        }

        return itemPrefabs[0]; // Default if no matching prefab found
    }

    private int GetPredominantSurroundingID(int x, int y, int z, int excludeID)
    {
        Dictionary<int, int> idCounts = new Dictionary<int, int>();
        List<Vector3Int> adjacentPositions = GetAdjacentPositions(x, y, z);

        foreach (var pos in adjacentPositions)
        {
            if (gridItems[pos.x, pos.y, pos.z] != null)
            {
                int id = GetPrefabID(gridItems[pos.x, pos.y, pos.z]);
                if (id != excludeID)
                {
                    int weight = targetItems.Any(item => item.prefab.GetComponent<Cell>().ID == id && item.targetCount > 0) ? 3 : 1;
                    if (idCounts.ContainsKey(id))
                    {
                        idCounts[id] += weight;
                    }
                    else
                    {
                        idCounts[id] = weight;
                    }
                }
            }
        }

        int predominantID = idCounts.OrderByDescending(pair => pair.Value).FirstOrDefault().Key;
        return predominantID != 0 ? predominantID : GetDifferentPrefabID(excludeID);
    }

    private int GetDifferentPrefabID(int excludeID)
    {
        // Check if only Rainbow Tiles are present
        bool onlyRainbowTiles = targetItems.All(item => item.isRainBowTile);

        // Create a list to store potential prefabs with their weights
        List<(GameObject prefab, int weight)> weightedPrefabs = new List<(GameObject prefab, int weight)>();

        foreach (var targetItem in targetItems)
        {
            if (GetPrefabID(targetItem.prefab) != excludeID && !targetItem.isRainBowTile)
            {
                int weight = targetItem.targetCount > 0 && !onlyRainbowTiles ? difficultyFactor : 1;
                weightedPrefabs.Add((targetItem.prefab, weight));
            }
        }

        // If there are no specific target items or only Rainbow Tiles, use regular item prefabs
        if (weightedPrefabs.Count == 0 || onlyRainbowTiles)
        {
            foreach (GameObject prefab in itemPrefabs)
            {
                if (GetPrefabID(prefab) != excludeID)
                {
                    weightedPrefabs.Add((prefab, 1));
                }
            }
        }

        // Sum of all weights
        int totalWeight = weightedPrefabs.Sum(item => item.weight);
        int randomIndex = Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var (prefab, weight) in weightedPrefabs)
        {
            sum += weight;
            if (randomIndex < sum)
            {
                return GetPrefabID(prefab);
            }
        }

        return GetPrefabID(weightedPrefabs.First().prefab); // Fallback in case of an error
    }

    private List<Vector3Int> GetAdjacentPositions(int x, int y, int z)
    {
        List<Vector3Int> adjacentPositions = new List<Vector3Int>();
        // Add positions up, down, left, right, front, back
        adjacentPositions.Add(new Vector3Int(x + 1, y, z));
        adjacentPositions.Add(new Vector3Int(x - 1, y, z));
        adjacentPositions.Add(new Vector3Int(x, y + 1, z));
        adjacentPositions.Add(new Vector3Int(x, y - 1, z));
        adjacentPositions.Add(new Vector3Int(x, y, z + 1));
        adjacentPositions.Add(new Vector3Int(x, y, z - 1));

        // Filter out invalid positions
        return adjacentPositions.Where(pos => IsValidPosition(pos.x, pos.y, pos.z)).ToList();
    }


    private bool IsValidPosition(int x, int y, int z)
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows || z < 0 || z >= depth || gridItems[x, y, z] != null)
        {
            return false;
        }

        // Check for diagonal matches
        if (IsDiagonalMatch(x, y, z))
        {
            return false;
        }

        // Check for T-shaped matches
        if (IsTShapeMatch(x, y, z))
        {
            return false;
        }

        return true;
    }

    private bool IsTShapeMatch(int x, int y, int z)
    {
        int? idAtCurrent = gridItems[x, y, z] != null ? (int?) GetPrefabID(gridItems[x, y, z]) : null;

        // Checking horizontal and vertical lines for a T-shape
        bool horizontalMatch = IsLineMatch(x - 1, y, z, 1, 0, idAtCurrent) &&
                               IsLineMatch(x, y - 1, z, 0, 1, idAtCurrent) &&
                               IsLineMatch(x, y + 1, z, 0, 1, idAtCurrent);
        bool verticalMatch = IsLineMatch(x, y - 1, z, 0, 1, idAtCurrent) &&
                             IsLineMatch(x - 1, y, z, 1, 0, idAtCurrent) && IsLineMatch(x + 1, y, z, 1, 0, idAtCurrent);

        return horizontalMatch || verticalMatch;
    }

    private bool IsLineMatch(int startX, int startY, int startZ, int dirX, int dirY, int? idToMatch)
    {
        int matchCount = 0;
        for (int i = 0; i < 2; i++)
        {
            int newX = startX + i * dirX;
            int newY = startY + i * dirY;
            if (IsValidGridPosition(newX, newY, startZ))
            {
                if (gridItems[newX, newY, startZ] != null && GetPrefabID(gridItems[newX, newY, startZ]) == idToMatch)
                {
                    matchCount++;
                }
            }
        }

        return matchCount == 2;
    }

    private bool IsDiagonalMatch(int x, int y, int z)
    {
        if (gridItems[x, y, z] != null)
        {
            int currentID = GetPrefabID(gridItems[x, y, z]);

            // Diagonal checks in the horizontal plane
            bool diagonalMatch1 = IsValidGridPosition(x + 1, y + 1, z) && IsValidGridPosition(x - 1, y - 1, z) &&
                                  GetPrefabID(gridItems[x + 1, y + 1, z]) == currentID &&
                                  GetPrefabID(gridItems[x - 1, y - 1, z]) == currentID;

            bool diagonalMatch2 = IsValidGridPosition(x + 1, y - 1, z) && IsValidGridPosition(x - 1, y + 1, z) &&
                                  GetPrefabID(gridItems[x + 1, y - 1, z]) == currentID &&
                                  GetPrefabID(gridItems[x - 1, y + 1, z]) == currentID;

            if (diagonalMatch1 || diagonalMatch2)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsValidGridPosition(int x, int y, int z)
    {
        // Check if the position is within the grid and not null
        return x >= 0 && x < columns && y >= 0 && y < rows && z >= 0 && z < depth && gridItems[x, y, z] != null;
    }

    private void GenerateGridWithIdentifiableCombinations()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (gridItems[x, y, z] == null)
                    {
                        int clusterSize = Random.Range(minClusterSize, maxClusterSize + 1);
                        int prefabIndex = Random.Range(0, itemPrefabs.Length);
                        int prefabID = GetPrefabID(itemPrefabs[prefabIndex]);

                        SpawnUShapeCluster(x, y, z, itemPrefabs[prefabIndex], prefabID, clusterSize);
                    }
                }
            }
        }
    }

    private void SpawnUShapeCluster(int startX, int startY, int startZ, GameObject prefab, int prefabID, int size)
    {
        HashSet<Vector3Int> spawnedPositions = new HashSet<Vector3Int>();
        Vector3Int currentPos = new Vector3Int(startX, startY, startZ);
        bool isHorizontal = Random.Range(0, 2) == 0; // Randomly choose between horizontal or vertical

        for (int i = 0; i < size; i++)
        {
            if (!IsValidPosition(currentPos.x, currentPos.y, currentPos.z))
                break;

            if (!spawnedPositions.Contains(currentPos))
            {
                SpawnItem(prefab, prefabID, currentPos);
                spawnedPositions.Add(currentPos);
            }

            // Move in the chosen direction (horizontal or vertical)
            if (isHorizontal)
            {
                currentPos.x += 1; // Move horizontally
            }
            else
            {
                currentPos.y += 1; // Move vertically
            }
        }
    }

    private void SpawnItem(GameObject prefab, int prefabID, Vector3Int position)
    {
        Vector3 worldPosition = new Vector3(position.x * spacing, position.y * spacing, position.z * spacing);
        GameObject newItem = Instantiate(prefab, worldPosition, Quaternion.identity, transform);
        newItem.GetComponent<Cell>().ID = prefabID;
        gridItems[position.x, position.y, position.z] = newItem;
        newItem.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack).From(Vector3.one / 10);
    }

    public int GetPrefabID(GameObject prefab)
    {
        return prefab.GetComponent<Cell>().ID;
    }

    private bool CheckIfCanSpawnBoosterTile()
    {
        bool canSpawn = GameplayUiManager.instance.currentSelections >=
                        GameplayUiManager.instance.selectionsToReachBooster;
        print("Can Spawn " + canSpawn);
        return canSpawn;
    }
}