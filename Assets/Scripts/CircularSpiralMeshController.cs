using UnityEngine;

[RequireComponent(typeof(CircularSpiralMeshGenerator))]
public class CircularSpiralMeshController : MonoBehaviour
{
    [Range(0, 20)]
    public float a;

    [Range(0, 20)]
    public float b;

    [Range(0, 1000)]
    public int numberOfVertices;

    [Range(0, 3)]
    public float thetaIncrement;

    [Range(0, 5)]
    public float width;

    [Range(0, 10)]
    public float thickness;

    private CircularSpiralMeshGenerator meshGenerator;

    private void Awake()
    {
        meshGenerator = GetComponent<CircularSpiralMeshGenerator>();
    }

    private void Start()
    {
        // Initialize parameters with current values
        a = meshGenerator.a;
        b = meshGenerator.b;
        numberOfVertices = meshGenerator.numberOfVertices;
        thetaIncrement = meshGenerator.thetaIncrement;
        width = meshGenerator.width;
        thickness = meshGenerator.thickness;
    }

    private void Update()
    {
        // Continuously update the mesh generator with the values from the inspector
        meshGenerator.a = a;
        meshGenerator.b = b;
        meshGenerator.numberOfVertices = numberOfVertices;
        meshGenerator.thetaIncrement = thetaIncrement;
        meshGenerator.width = width;
        meshGenerator.thickness = thickness;
        UpdateMesh();
    }
    /// <summary>
    /// Updates the mesh generator to create the circular spiral mesh.
    /// </summary>
    public void UpdateMesh()
    {
        meshGenerator.UpdateMesh();
    }
}


