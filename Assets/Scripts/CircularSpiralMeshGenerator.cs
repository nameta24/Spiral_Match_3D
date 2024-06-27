using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CircularSpiralMeshGenerator : MonoBehaviour
{
    // Parameters for generating the circular spiral mesh
    public float a = 0.2f;
    public float b = 0.2f;
    public int numberOfVertices = 200;
    public float thetaIncrement = 0.05f;
    public float width = 0.1f;
    public float thickness = 0.1f;

    /// <summary>
    /// Initializes the circular spiral mesh by generating and setting it up at the start of the object.
    /// </summary>
    /// <returns>Returns nothing.</returns>

    private void Start()
    {
        // Generate and set the circular spiral mesh
        
        GetComponent<MeshFilter>().mesh = GenerateCircularSpiralMesh();
    }
    /// <summary>
    /// Generates a circular spiral mesh based on specified parameters.
    /// </summary>
    /// <returns>The generated mesh.</returns>

    private Mesh GenerateCircularSpiralMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        float theta = 0f;

        for (int i = 0; i < numberOfVertices; i++)
        {
            float r = a + b * theta;

            // Define vertices for the inner and outer surfaces of the spiral
            Vector3 innerBottomVertex = new Vector3(r * Mathf.Cos(theta), -thickness / 2, r * Mathf.Sin(theta));
            Vector3 outerBottomVertex = new Vector3((r + width) * Mathf.Cos(theta), -thickness / 2, (r + width) * Mathf.Sin(theta));
            Vector3 innerTopVertex = new Vector3(r * Mathf.Cos(theta), thickness / 2, r * Mathf.Sin(theta));
            Vector3 outerTopVertex = new Vector3((r + width) * Mathf.Cos(theta), thickness / 2, (r + width) * Mathf.Sin(theta));

            int baseIndex = 8 * i;

            // Add bottom and top vertices to the lists
            verticesList.AddRange(new[] { innerBottomVertex, outerBottomVertex, innerBottomVertex, outerBottomVertex });
            verticesList.AddRange(new[] { innerTopVertex, outerTopVertex, innerTopVertex, outerTopVertex });

            float uCoord = (float)i / (numberOfVertices - 1);

            // UVs
            for (int j = 0; j < 8; j++)
            {
                uvsList.Add(new Vector2(uCoord, j % 2 == 0 ? 0 : 1));
            }

            if (i != numberOfVertices - 1)
            {
                trianglesList.AddRange(new[]
                {
                    baseIndex, baseIndex + 1, baseIndex + 8,
                    baseIndex + 8, baseIndex + 1, baseIndex + 9,
                    baseIndex + 4, baseIndex + 12, baseIndex + 5,
                    baseIndex + 5, baseIndex + 12, baseIndex + 13,
                    baseIndex + 2, baseIndex + 10, baseIndex + 6,
                    baseIndex + 6, baseIndex + 10, baseIndex + 14,
                    baseIndex + 3, baseIndex + 7, baseIndex + 11,
                    baseIndex + 11, baseIndex + 7, baseIndex + 15
                });
            }

            theta += thetaIncrement;
        }

        // Adding the start and end caps to the mesh
        Vector3 startInnerBottom = verticesList[0];
        Vector3 startOuterBottom = verticesList[1];
        Vector3 startInnerTop = verticesList[4];
        Vector3 startOuterTop = verticesList[5];

        Vector3 endInnerBottom = verticesList[8 * (numberOfVertices - 1)];
        Vector3 endOuterBottom = verticesList[8 * (numberOfVertices - 1) + 1];
        Vector3 endInnerTop = verticesList[8 * (numberOfVertices - 1) + 4];
        Vector3 endOuterTop = verticesList[8 * (numberOfVertices - 1) + 5];

        // Add vertices and UVs for start and end caps
        verticesList.AddRange(new[] { startInnerBottom, startOuterBottom, startInnerTop, startOuterTop });
        uvsList.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) });

        verticesList.AddRange(new[] { endInnerBottom, endOuterBottom, endInnerTop, endOuterTop });
        uvsList.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) });

        // Define triangles for start and end caps
        int startCapIndex = 8 * numberOfVertices;
        trianglesList.AddRange(new[]
        {
            startCapIndex, startCapIndex + 2, startCapIndex + 1,
            startCapIndex + 1, startCapIndex + 2, startCapIndex + 3
        });

        int endCapIndex = startCapIndex + 4;
        trianglesList.AddRange(new[]
        {
            endCapIndex, endCapIndex + 1, endCapIndex + 2,
            endCapIndex + 1, endCapIndex + 3, endCapIndex + 2
        });

        // Assign vertices, UVs, and triangles to the mesh
        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvsList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
    /// <summary>
    /// Updates the circular spiral mesh with new parameters.
    /// </summary>
    /// <returns>Returns nothing.</returns>

    public void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = GenerateCircularSpiralMesh();
    }
    /// <summary>
    /// Sets the color of the circular spiral mesh.
    /// </summary>
    /// <param name="color">The color to be applied to the mesh.</param>
    /// <returns>Returns nothing.</returns>

    public void SetMeshColor(Color color)
    {
        Material material = GetComponent<MeshRenderer>().material;
        material.color = color;
    }
}