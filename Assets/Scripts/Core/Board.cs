using UnityEngine;
using System.Collections;
public class Board : MonoBehaviour
{
    public Transform emptySprite;
    public int height, width;

    public int header = 5;
    private Transform[,] grid;
    public int completedRows = 0;

    public ParticlePlayer[] rowGlowFX = new ParticlePlayer[4];

    private void Awake()
    {
        grid = new Transform[width, height];
    }

    private void Start()
    {
        DrawEmptyCells();
    }

    private void DrawEmptyCells()
    {
        if (emptySprite)
        {
            for (int y = 0; y < height - header; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Transform clone;
                    clone = Instantiate(emptySprite, new Vector3(x, y, 0), Quaternion.identity);
                    clone.name = "EmptyCell(" + x + "," + y + ")";
                    clone.transform.parent = transform;
                }
            }
        }
    }

    private bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0);
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach (Transform s in shape.transform)
        {
            Vector2 pos = VectorF.Round(s.position);
            bool a = IsWithinBoard((int)pos.x, (int)pos.y);
            if (!a) return false;

            bool b = IsOccupied((int)pos.x, (int)pos.y, shape);
            if (b) return false;
        }

        return true;
    }

    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null)
        {
            return;
        }

        foreach (Transform t in shape.transform)
        {
            Vector2 pos = VectorF.Round(t.position);
            grid[(int)pos.x, (int)pos.y] = t;
        }
    }

    public bool IsOccupied(int x, int y, Shape shape)
    {
        return (grid[x, y] != null && grid[x, y].parent != shape.transform);
    }

    private bool IsComplete(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
                Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    private void ShiftRowDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    private void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < height; i++)
        {
            ShiftRowDown(i);
        }
    }

    public IEnumerator ClearAllRows()
    {
        completedRows = 0;

        for (int y = 0; y < height; ++y)
        {
            if (IsComplete(y))
            {
                ClearRowEffect(completedRows, y);
                completedRows++;
            }
        }

        yield return new WaitForSeconds(0.3f);

        for (int y = 0; y < height; y++)
        {
            if (IsComplete(y))
            {
                ClearRow(y);
                ShiftRowsDown(y + 1);
                yield return new WaitForSeconds(.2f);
                y--;
            }
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y >= (height - header - 1))
            {
                return true;
            }
        }

        return false;
    }

    void ClearRowEffect(int index, int y)
    {
        if (rowGlowFX[index])
        {
            rowGlowFX[index].transform.position = new Vector3(0, y, -5);
            rowGlowFX[index].Play();
        }
    }
}