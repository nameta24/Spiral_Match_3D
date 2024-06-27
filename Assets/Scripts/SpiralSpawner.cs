using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class SpiralSpawner : MonoBehaviour
{
    public GameObject spiralPrefab;
    public void SpawnSpiral(Color color, int verticesCount , Vector3 spawnPos,GameObject tileObject)
    {
        GameObject spiral = Instantiate(spiralPrefab, spawnPos, Quaternion.identity);
        CircularSpiralMeshController controller = spiral.GetComponent<CircularSpiralMeshController>();
        
        // Set the color of the spiral.
        if (controller)
        {
            controller.GetComponent<CircularSpiralMeshGenerator>().SetMeshColor(color);
        }

        // Set the vertices count smoothly.
        UpdateSpiralVerticesSmoothly(controller, verticesCount, 0.25f);

        // Perform any additional setup or animations on your spiral here.
        spiral.transform.DOScale(Vector3.one, .3f).From(Vector3.zero).SetEase(Ease.OutBack);
        spiral.transform.DORotate(new Vector3(90, 0, 0), 0.3f, RotateMode.FastBeyond360);
        MoveSpiralsAwayFromGrid(spiral, tileObject);
    }

    void  MoveSpiralsAwayFromGrid(GameObject spiral,GameObject tileObject)
    {
        bool isAlsoTargetItem = false;

        foreach (GridGenerator3D.TargetItem ti in GridGenerator3D.instance.targetItems)
        {
            if (tileObject.GetComponent<Cell>().ID==ti.prefab.GetComponent<Cell>().ID)
            {
              ti.targetCount--;
              if (ti.targetCount<=0)
              {
                  ti.targetCount = 0;
              }
                isAlsoTargetItem = true;
            }
        }
        Vector3 targetPosition = LevelManager.instance.ReturnTransformToMoveSpiralTo(tileObject).position;
     //   yield return new WaitForSeconds(0.4f);

        if (isAlsoTargetItem)
        {
            float moveDuration = 0.6f; 
            Vector3 targetScale = new Vector3(0, 0, 0); 

            spiral.transform.DORotate(new Vector3(90,0,0), moveDuration/2).SetEase(Ease.OutBack);

            spiral.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InBack).SetDelay(.3f);

            spiral.transform.DOScale(targetScale, moveDuration/2).SetEase(Ease.Linear).SetDelay(moveDuration/1.2f+.3f).OnComplete(() =>
            {
                LevelManager.instance.UpdateTargetItemUI(GridGenerator3D.instance.targetItems.ToList());
                LinkerStatic.instance.CheckWinningCondition();
                MyHapticsManager.Instance.PlayMediumHaptics();
            });
           // flyTowardsTarget.PlayFeedbacks();
           AudioManager.Instance.PlaySound(AudioManager.Instance.flyTowardsTarget);

        }
        else
        {
            Rigidbody rb = spiral.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            spiral.transform.DORotate(new Vector3(-180,0,0), .5f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Incremental);
            LinkerStatic.instance.CheckWinningCondition();
            AudioManager.Instance.PlaySound(AudioManager.Instance.spiralFall);
        

        }
       Destroy(spiral,2.5f);
        
    }

   

    private void UpdateSpiralVerticesSmoothly(CircularSpiralMeshController controller, int targetVertices, float duration)
    {
        int startVertices = controller.numberOfVertices;
        DOTween.To(() => startVertices, x => controller.numberOfVertices = x, targetVertices, duration)
            .OnUpdate(controller.UpdateMesh)
            .SetEase(Ease.Linear);
    }
}