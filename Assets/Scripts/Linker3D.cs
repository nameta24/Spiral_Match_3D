using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Linker3D : MonoBehaviour
{
    public LayerMask itemLayer;

    private List<GameObject> linkedItems = new List<GameObject>();
    private Color initialColor;
    public GameObject spiralPrefab;
    public float spiralMoveOutFactor = 0.2f;
    private GameObject activeSpiral;
    private CircularSpiralMeshController spiralController;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLink();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueLink();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndLink();
        }

        if (activeSpiral)
        {
            float zAdjustment = Mathf.Lerp(0, -spiralMoveOutFactor * linkedItems.Count, 0.5f);
            activeSpiral.transform.position = new Vector3(activeSpiral.transform.position.x,
                activeSpiral.transform.position.y, zAdjustment);
        }
    }

    private void StartLink()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, itemLayer))
        {
            linkedItems.Add(hit.collider.gameObject);
            initialColor = hit.collider.GetComponent<Renderer>().material.color;

            hit.collider.gameObject.SetActive(false);

            Vector3 spawnPos = hit.collider.transform.position + new Vector3(0, hit.collider.bounds.extents.y, 0);
            activeSpiral = Instantiate(spiralPrefab, spawnPos, Quaternion.identity);
            spiralController = activeSpiral.GetComponent<CircularSpiralMeshController>();
            activeSpiral.GetComponent<CircularSpiralMeshGenerator>().SetMeshColor(initialColor);

            activeSpiral.transform.up = Vector3.up;
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

            // Check if the new object is adjacent to the last object
            float distance = Vector3.Distance(newObject.transform.position, lastObject.transform.position);
            bool isAdjacent = (Mathf.Approximately(newObject.transform.position.x, lastObject.transform.position.x) ||
                               Mathf.Approximately(newObject.transform.position.y, lastObject.transform.position.y) ||
                               Mathf.Approximately(newObject.transform.position.z, lastObject.transform.position.z)) &&
                              Mathf.Approximately(distance, GridGenerator3D.instance.spacing);

            // Check if the new object's color matches the initial color and if it is adjacent
            if (!linkedItems.Contains(newObject) && isAdjacent &&
                newObject.GetComponent<Renderer>().material.color == initialColor)
            {
                linkedItems.Add(newObject);
                newObject.SetActive(false);

                // Update spiral position and orientation
                UpdateSpiralPositionAndOrientation(newObject, lastObject);
            }
            // Else, break the link if a different color is encountered
            else if (newObject.GetComponent<Renderer>().material.color != initialColor)
            {
                EndLink(); // End the linking process
                return; // Exit the method to avoid further processing
            }
        }

        // Update spiral controller if it exists
        if (spiralController)
        {
            spiralController.numberOfVertices = linkedItems.Count * 20;
            spiralController.UpdateMesh();
        }
    }

    private void UpdateSpiralPositionAndOrientation(GameObject newObject, GameObject lastObject)
    {
        Vector3 targetPos = newObject.transform.position + new Vector3(0,
            newObject.GetComponent<Collider>().bounds.extents.y, activeSpiral.transform.position.z);
        activeSpiral.transform.DOMove(targetPos, 0.2f);

        if (!Mathf.Approximately(newObject.transform.position.x, lastObject.transform.position.x))
        {
            activeSpiral.transform.DORotate(new Vector3(0, 90, 0), 0.3f);
        }
        else if (!Mathf.Approximately(newObject.transform.position.y, lastObject.transform.position.y))
        {
            activeSpiral.transform.DORotate(new Vector3(0, 0, 90), 0.3f);
        }
    }

    private void EndLink()
    {
        if (linkedItems.Count >= 3)
        {
            foreach (GameObject item in linkedItems)
            {
                Destroy(item);
            }

            if (activeSpiral)
            {
                Rigidbody rb = activeSpiral.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;

                activeSpiral = null;
            }
        }
        else
        {
            foreach (GameObject item in linkedItems)
            {
                item.SetActive(true);
            }

            if (activeSpiral)
            {
                Destroy(activeSpiral);
                activeSpiral = null;
            }
        }

        linkedItems.Clear();
    }
}