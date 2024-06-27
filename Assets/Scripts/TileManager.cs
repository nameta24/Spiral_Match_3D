using DG.Tweening;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private GridGenerator3D gridGenerator;

    private void Start()
    {
        gridGenerator = GetComponent<GridGenerator3D>();
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator3D component not found on the GameObject.");
        }
    }

    public void MoveTilesDownBeforeRespawn()
    {
        MoveTilesDown();
    }

    private void MoveTilesDown()
    {
        for (int x = 0; x < gridGenerator.columns; x++)
        {
            for (int z = 0; z < gridGenerator.depth; z++)
            {
                for (int y = 0; y < gridGenerator.rows; y++)
                {
                    if (gridGenerator.gridItems[x, y, z] == null)
                    {
                        MoveTileDown(x, y, z);
                    }
                }
            }
        }
    }

    private void MoveTileDown(int x, int startY, int z)
    {
        // Start from the top and move down the column
        for (int y = startY; y < gridGenerator.rows - 1; y++)
        {
            if (gridGenerator.gridItems[x, y, z] == null)
            {
                // Find the first non-empty tile above the current position
                int nonEmptyY = FindFirstNonEmptyPositionAbove(x, y, z);
                if (nonEmptyY != -1)
                {
                    // Move the tile down to the current position
                    gridGenerator.gridItems[x, y, z] = gridGenerator.gridItems[x, nonEmptyY, z];
                    gridGenerator.gridItems[x, nonEmptyY, z] = null;

                    // Calculate the new position
                    Vector3 newPosition = new Vector3(x * gridGenerator.spacing, y * gridGenerator.spacing,
                        z * gridGenerator.spacing);

                    // Animate the tile moving down using DOTween
                    gridGenerator.gridItems[x, y, z].transform.DOMove(newPosition, 0.3f).SetEase(Ease.OutBack);
                }
            }
        }
    }

    private int FindFirstNonEmptyPositionAbove(int x, int currentY, int z)
    {
        for (int y = currentY + 1; y < gridGenerator.rows; y++)
        {
            if (gridGenerator.gridItems[x, y, z] != null)
            {
                return y; // Return the position of the first non-empty tile above
            }
        }

        return -1; // No non-empty position found above
    }
}

