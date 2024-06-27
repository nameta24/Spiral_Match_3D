/// <summary>
/// This script manages the linking of game objects in a spiral pattern based on mouse input.
/// </summary>
/// <returns>Returns nothing.</returns>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


public class LinkerStatic : MonoBehaviour
{
    public static LinkerStatic instance;
    public AudioSource endSoundAudioSource;
    public LevelManager levelManager;
    public LayerMask itemLayer;
    private List<GameObject> linkedItems = new List<GameObject>();
    private Color initialColor;
    public GameObject spiralPrefab;
    private bool _spiralState, inputPause;
    private bool _ifTargetItem;
    public float spiralMoveOutFactor; //Variable for rolling spiral outside of the Grid
    private int spiralCompletionCount = 0;
    public float zOffset = 1;
    //private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();


    private GameObject activeSpiral;
    private CircularSpiralMeshController spiralController;

    private bool isSpiralPhysicsApplied = true;

    public static int maxSpiralCount;
    [HideInInspector] public int remainingMoves;

    [FormerlySerializedAs("spiralCountText")] [SerializeField]
    private TMP_Text movesText; // Reference to the TextMeshProUGUI component

    public AudioSource linkClip; // Reference to the AudioSource component for link sound
    public AudioSource endLinkClip; // Reference to the AudioSource component for end link sound

    private AudioSource currentAudioSource;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Initializes spiral counts and UI text on start.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    void Start()
    {
       
        currentAudioSource = linkClip;
        _ifTargetItem = false;
    }

    public void SetupMoves()
    {
        remainingMoves = maxSpiralCount;
        UpdateMovesCount();
    }

    /// <summary>
    /// Handles mouse input for initiating, continuing, or ending the linking process.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    private void Update()
    {
        if (levelManager.hasWon || levelManager.hasFailed || inputPause)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            TakeInputInEditor();
        }
        else
        {
            TakeMobileInput();
        }
    }

    void TakeMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch is over a UI element
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        StartLink();
                        break;
                    case TouchPhase.Moved:
                        ContinueLink();
                        break;
                    case TouchPhase.Ended:
                        EndLink();
                        break;
                }
            }
        }
    }

    void TakeInputInEditor()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartLink();
        }
        else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ContinueLink();
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            EndLink();
        }
    }

    /// <summary>
    /// Initiates linking process upon mouse click.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    private void StartLink()
    {
        if (_spiralState) return;
        if (activeSpiral && !isSpiralPhysicsApplied || inputPause) return;
        // Play the click sound if AudioSource and clip are set


        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, itemLayer))
        {
            linkedItems.Add(hit.collider.gameObject);
            initialColor = hit.collider.GetComponent<Renderer>().material.color;
            hit.collider.GetComponent<Cell>().ShowSelectionParticles();
            //active state of highlight on prefab
            //  hit.collider.GetComponent<Renderer>().material.color = initialColor * 1.2f;
            hit.collider.transform.GetChild(0).gameObject.SetActive(true);
            hit.collider.transform.GetComponent<DOTweenAnimation>().DORestartById("Select");

            AudioManager.Instance.PlaySound(AudioManager.Instance.selectionSound);

            GameplayUiManager.instance.currentSelections++;
            MyHapticsManager.Instance.PlayMediumHaptics();
        }
    }

    private void ContinueLink()
    {
        if (linkedItems.Count == 0) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, itemLayer))
        {
            GameObject newObject = hit.collider.gameObject;
            GameObject lastObject = linkedItems[linkedItems.Count - 1];

            float distance = Vector3.Distance(newObject.transform.position, lastObject.transform.position);
            bool isAdjacent =
                (Mathf.Approximately(newObject.transform.position.x, lastObject.transform.position.x) ||
                 Mathf.Approximately(newObject.transform.position.y, lastObject.transform.position.y) ||
                 Mathf.Approximately(newObject.transform.position.z, lastObject.transform.position.z)) &&
                Mathf.Approximately(distance, GridGenerator3D.instance.spacing);

            GameObject secondLastObject = (linkedItems.Count > 1) ? linkedItems[linkedItems.Count - 2] : null;

            if (!linkedItems.Contains(newObject) && isAdjacent)
            {
                if (newObject.GetComponent<Renderer>().material.color == initialColor)
                {
                    linkedItems.Add(newObject);
                    hit.collider.GetComponent<Cell>().ShowSelectionParticles();

                    //active state of highlight on prefab
                    // newObject.GetComponent<Renderer>().material.color = initialColor * 1.2f;
                    newObject.transform.GetChild(0).gameObject.SetActive(true);
                    AudioManager.Instance.PlaySound(AudioManager.Instance.selectionSound);

                    hit.collider.transform.GetComponent<DOTweenAnimation>().DORestartById("Select");
                    MyHapticsManager.Instance.PlayMediumHaptics();
                    GameplayUiManager.instance.currentSelections++;
                }
            }
            else if (secondLastObject && newObject == secondLastObject)
            {
                lastObject.GetComponent<Renderer>().material.color = initialColor;
                lastObject.transform.GetChild(0).gameObject.SetActive(false);
                hit.collider.transform.GetComponent<DOTweenAnimation>().DORestartById("Select");

                AudioManager.Instance.PlaySound(AudioManager.Instance.selectionSound);

                GameplayUiManager.instance.currentSelections--;
                MyHapticsManager.Instance.PlayMediumHaptics();
                linkedItems.RemoveAt(linkedItems.Count - 1);
            }
        }
    }

    /// <summary>
    /// Ends the linking process and creates a spiral if conditions are met.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    private void EndLink()
    {
        if (_spiralState)
        {
            return;
        }

        if (activeSpiral && !isSpiralPhysicsApplied)
        {
            return;
        }

        MyHapticsManager.Instance.PlayMediumHaptics();

        if (linkedItems.Count >= 3)
        {
            inputPause = true;
            UpdateTargetItemCounts(linkedItems);
            if (spiralCompletionCount >= maxSpiralCount - 1)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.selectionSound);
            }

            if (linkedItems.Count < GameplayUiManager.instance.selectionsToReachBooster)
            {
                GameplayUiManager.instance.currentSelections = 0;
            }

            StartCoroutine(MoveSpiralThroughTiles());
        }
        else
        {
            foreach (GameObject item in linkedItems)
            {
                item.GetComponent<Renderer>().material.color = initialColor;
                item.transform.GetChild(0).gameObject.SetActive(false);
                // Destroy(item.gameObject);
            }

            linkedItems.Clear();
            if (GameplayUiManager.instance.currentSelections > 0)
            {
                GameplayUiManager.instance.currentSelections--;
            }
        }
    }


    /// <summary>
    /// updates the target
    /// </summary>
    /// <param name="matchedItems"></param>
    public void UpdateTargetItemCounts(List<GameObject> matchedItems)
    {
        foreach (var targetItem in GridGenerator3D.instance.targetItems)
        {
            int matchedCount = CountMatchedItems(matchedItems, targetItem.prefab);
            targetItem.targetCount -= matchedCount;

            // Prevent target count from going negative
            targetItem.targetCount = Mathf.Max(targetItem.targetCount, 0);
        }
    }

    /// <summary>
    /// counts matched items
    /// </summary>
    /// <param name="matchedItems"></param>
    /// <param name="targetPrefab"></param>
    /// <returns></returns>
    private int CountMatchedItems(List<GameObject> matchedItems, GameObject targetPrefab)
    {
        
        string targetItemName = GetBasePrefabName(targetPrefab);
        int count = 0;

        foreach (var matchedItem in matchedItems)
        {
            string matchedItemName = GetBasePrefabName(matchedItem);

            if (targetItemName == matchedItemName )
            {
                count++;
                _ifTargetItem = true;
            }
        }

        return count;
    }

    /// <summary>
    /// gets prefabs name to match if it is same as target
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private string GetBasePrefabName(GameObject prefab)
    {
        string prefabName = prefab.name;
        int cloneIndex = prefabName.IndexOf("(Clone)");

        return (cloneIndex == -1) ? prefabName : prefabName.Substring(0, cloneIndex).Trim();
    }

    /// <summary>
    /// checks for win condition
    /// </summary>
    public void CheckWinningCondition()
    {
        bool allTargetsReached = GridGenerator3D.instance.targetItems.All(item => item.targetCount <= 0);

        if (allTargetsReached)
        {
            levelManager.LevelComplete();
        }
    }

    /// <summary>
    /// Coroutine to move a spiral through linked objects.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    private IEnumerator MoveSpiralThroughTiles()
    {
        _spiralState = true;

        isSpiralPhysicsApplied = false;

        GameObject firstTile = linkedItems[0];
        Vector3 spawnPos = firstTile.transform.position +
                           new Vector3(0, firstTile.GetComponent<Collider>().bounds.extents.y, -zOffset);

        firstTile.SetActive(false);
        Vector3 targetPosition = levelManager.targetItemsParent.position;
        targetPosition = levelManager.ReturnTransformToMoveSpiralTo(firstTile).position;

        Destroy(firstTile, 0.5f);


        activeSpiral = Instantiate(spiralPrefab, spawnPos, Quaternion.identity);
        spiralController = activeSpiral.GetComponent<CircularSpiralMeshController>();
        activeSpiral.GetComponent<CircularSpiralMeshGenerator>().SetMeshColor(initialColor);
        activeSpiral.transform.DOScale(Vector3.one, .6f).From(Vector3.zero).SetEase(Ease.OutBack);

        float growFactorVertices = 20;

        for (int i = 0; i < linkedItems.Count; i++)
        {
            GameObject tile = linkedItems[i];


            Vector3 targetPos = new Vector3(tile.transform.position.x, tile.transform.position.y,
                -spiralMoveOutFactor * (i + 1) /
                2); //Logic (-spiralMoveOutFactor*i) for rolling spiral outside of the Grid

            GameObject lastTile = (i > 0) ? linkedItems[i - 1] : null;

            if (lastTile)
            {
                if (!Mathf.Approximately(tile.transform.position.x, lastTile.transform.position.x))
                {
                    activeSpiral.transform.DORotate(new Vector3(0, 90, 0), 0.3f, RotateMode.FastBeyond360);
                }
                else if (!Mathf.Approximately(tile.transform.position.y, lastTile.transform.position.y))
                {
                    activeSpiral.transform.DORotate(new Vector3(0, 0, 90), 0.3f, RotateMode.FastBeyond360);
                }
            }

            float targetVertices = (i + 1) * growFactorVertices;
            UpdateSpiralVerticesSmoothly(spiralController, (int) targetVertices, 0.25f);

            Tween moveTween = activeSpiral.transform.DOMove(targetPos, 0.13f).SetEase(Ease.Linear);

            yield return moveTween.WaitForCompletion();

            if (i > 0 && tile)
            {
                //  tile.SetActive(false);
                tile.GetComponent<Cell>().ShowSelectionParticles();
                Destroy(tile);
                AudioManager.Instance.PlaySound(AudioManager.Instance.spiralFormation);

                MyHapticsManager.Instance.PlayMediumHaptics();
            }
        }

        if (_ifTargetItem == true)
        {
            float moveDuration = 0.6f; // Time taken to reach the top
            // float targetY = Camera.main.ViewportToWorldPoint(new Vector3(0, -1, 0)).z; // Y coordinate of top screen edge
            Vector3 targetScale = new Vector3(0, 0, 0); // Target scale when moving upwards

            activeSpiral.transform.DORotate(new Vector3(90, 0, 0), moveDuration / 2).SetEase(Ease.OutBack);

            // Move the activeSpiral to the target position
            activeSpiral.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InBack);

            // Scale down the activeSpiral during the move
            activeSpiral.transform.DOScale(targetScale, moveDuration / 2).SetEase(Ease.Linear)
                .SetDelay(moveDuration / 1.2f).OnComplete(() =>
                {
                    // Update the UI in the Level Manager
                    levelManager.UpdateTargetItemUI(GridGenerator3D.instance.targetItems.ToList());
                    MyHapticsManager.Instance.PlayMediumHaptics();
                    CheckWinningCondition();
                });
            AudioManager.Instance.PlaySound(AudioManager.Instance.flyTowardsTarget);

        }
        else
        {
            Rigidbody rb = activeSpiral.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            activeSpiral.transform.DORotate(new Vector3(-180, 0, 0), .5f).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
            AudioManager.Instance.PlaySound(AudioManager.Instance.spiralFall);

            CheckWinningCondition();
        }

        StartCoroutine(Delay(activeSpiral));
        _ifTargetItem = false;
        activeSpiral = null;
        isSpiralPhysicsApplied = true;

        linkedItems.Clear();

        spiralCompletionCount++;
        remainingMoves = maxSpiralCount - spiralCompletionCount;
        UpdateMovesCount();

        if (spiralCompletionCount >= maxSpiralCount)
        {
            DestroyGridAndSpawnNewObject();
            if (endSoundAudioSource != null && endSoundAudioSource.clip != null)
            {
                endSoundAudioSource.Play();
            }
        }
        GridGenerator3D.instance.MoveDownAndReSpawn();

        _spiralState = false;

        inputPause = false;
    }

    IEnumerator Delay(GameObject spiral)
    {
        yield return new WaitForSeconds(2f);
        spiral.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the UI text displaying remaining spiral count.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    private void UpdateMovesCount()
    {
        if (movesText != null)
        {
            movesText.text = remainingMoves.ToString();
            movesText.GetComponent<DOTweenAnimation>().DORestartById("Feedback");
        }
    }

    /// <summary>
    /// Destroys all objects in the scene and spawns a new one.
    /// </summary>
    /// <returns>Returns nothing.</returns>
    private void DestroyGridAndSpawnNewObject()
    {
        levelManager.LevelFail();
        spiralCompletionCount = 0;
        //restart functionality
        //remainingSpiralCount = maxSpiralCount;
        //UpdateSpiralCountText();
    }

    /// <summary>
    /// Updates the vertices of a spiral smoothly using DoTween animations.
    /// </summary>
    /// <param name="controller">The CircularSpiralMeshController reference.</param>
    /// <param name="targetVertices">The target number of vertices.</param>
    /// <param name="duration">The duration of the update animation.</param>
    /// <returns>Returns nothing.</returns>
    private void UpdateSpiralVerticesSmoothly(CircularSpiralMeshController controller, int targetVertices,
        float duration)
    {
        int startVertices = controller.numberOfVertices;
        DOTween.To(() => startVertices, x => controller.numberOfVertices = x, targetVertices, duration)
            .OnUpdate(controller.UpdateMesh)
            .SetEase(Ease.Linear);
    }
}