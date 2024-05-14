using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GridManager : MonoBehaviour
{
    [SerializeField] int height;
    [SerializeField] int width;
    [SerializeField] float cellSize;
    [SerializeField] GameObject[] cellPrefabs;

    public Cell[,] cells;
    [SerializeField] List<Cell> flatCells = new List<Cell>();
    public GridManager(int height, int width, float cellSize, GameObject[] cellPrefabs)
    {
        this.height = height;
        this.width = width;
        this.cellSize = cellSize;
        this.cellPrefabs = cellPrefabs;

        GenerateGrid();
    }
    void UpdateCellsInfo()
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                UpdateCellInfo(x, y);
            }
        }
    }
    void Awake()
    {
        ConvertFlatArrayTo2D();
    }

    private void ConvertFlatArrayTo2D()
    {
        if (flatCells != null && flatCells.Count > 0)
        {
            InitializeCells();
            for (int i = 0; i < flatCells.Count; i++)
            {
                int x = i / height;
                int y = i % height;
                cells[x, y] = flatCells[i];
                cells[x, y].SetXY(x, y);
            }
        }
    }


    public void InitializeCells()
    {
        cells = new Cell[width, height];
    }

    [ContextMenu("GenerateGrid")] //allows calling function from editor 
    void GenerateGrid()
    {
        ResetGrid();
        InitializeCells();
        flatCells = new List<Cell>();

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                GenerateCell(x, y);
            }
        }
        UpdateCellsInfo();
    }

    void GenerateCell(int x, int y)
    {
        int randomCellPrefabIndex = Random.Range(0, cellPrefabs.Length - 1);
        GameObject cellVisual = PrefabUtility.InstantiatePrefab(cellPrefabs[randomCellPrefabIndex]) as GameObject;
        // GameObject cellVisual = Instantiate(cellPrefabs[randomCellPrefabIndex]);

        cellVisual.transform.SetParent(this.transform);
        cellVisual.transform.position = GetWorldPosition(x, y);
        cellVisual.transform.rotation = Quaternion.identity;
        cellVisual.name = x + "/" + y + " " + "Cell";
        Cell cell = cellVisual.GetComponent<Cell>();
        cellVisual.isStatic = true;
        cells[x, y] = cell;
        flatCells.Add(cell);

    }


    void UpdateCellInfo(int x, int y)
    {
        cells[x, y].SetXY(x, y);
        cells[x, y].SetGridManager(this);
    }

    //[ContextMenu("UpdateGrid")] don't use this for now
    void UpdateGrid()
    {
        if (cells == null) ConvertFlatArrayTo2D();
        Cell[,] tempCopy = cells.Clone() as Cell[,]; // create backup of grid

        InitializeCells(); // expand/shrink array


        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (x < tempCopy.GetLength(0) && y < tempCopy.GetLength(1))
                {
                    // Transfer cell from the old grid if within bounds of the new grid.
                    cells[x, y] = tempCopy[x, y];
                }
                else
                {
                    // Create a new cell if the position is new in the grid.
                    GenerateCell(x, y);
                }
            }
        }

        // Destroy any cells that were in the old grid but not transferred to the new grid.
        for (int x = 0; x < tempCopy.GetLength(0); x++)
        {
            for (int y = 0; y < tempCopy.GetLength(1); y++)
            {
                if (x >= width || y >= height)
                {
                    // Check if the cell exists before destroying it.
                    if (tempCopy[x, y] != null)
                    {
                        flatCells.Remove(tempCopy[x, y]);
                        DestroyImmediate(tempCopy[x, y].gameObject);
                    }

                }
            }
        }

        UpdateCellsInfo();
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        return transform.TransformPoint(new Vector3(x, 0, y) * cellSize);// transforms the position from global to local position
    }
    public bool GetGridIndexes(Vector2Int initialIndex, int width, int height, out List<Vector2Int> indexes)
    {
        indexes = new List<Vector2Int>();

        // Check if the requested grid area is out of the overall grid bounds
        if (initialIndex.x + width > cells.GetLength(0) || initialIndex.y + height > cells.GetLength(1))
        {
            Debug.Log("Requested grid area is out of bounds.");
            return false;
        }

        // Populate the list of indexes within the specified sub-area
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int index = initialIndex + new Vector2Int(x, y);
                if (cells[index.x, index.y] == null) // Check if the cell at this index is null
                {
                    Debug.Log("Null cell encountered at index: " + index);
                    indexes.Clear(); // Clear the list since the operation failed
                    return false;
                }
                indexes.Add(index); // Add the coordinate to the list
            }
        }
        return true; // All checks passed, and the list of indexes is populated
    }
    public bool GetGrids(Vector2Int initialIndex, int width, int height, out List<Cell> cells)
    {
        List<Vector2Int> indexes;
        bool insideArray = GetGridIndexes(initialIndex, width, height, out indexes);

        cells = new List<Cell>();
        if (!insideArray) return false;

        foreach (Vector2Int i in indexes)
        {
            if (this.cells[i.x, i.y] == null) return false;
            cells.Add(this.cells[i.x, i.y]);
        }
        return true;
    }
    public Cell GetCellFromIndex(int x, int y)
    {
        if (x < 0 || y < 0 || x < width || y < height)
            return null;
        else
            return cells[x, y];
    }

    public void OnBeforeSerialize()
    {
        if (cells == null) return;
        flatCells.Clear();
        foreach (Cell cell in cells)
        {
            flatCells.Add(cell);
        }
    }

    public void OnAfterDeserialize()
    {
        ConvertFlatArrayTo2D();
    }
    [ContextMenu("TestCells")]
    void TestCells()
    {
        foreach (Cell cell in cells)
        {
            Debug.Log(cell.name);
        }
    }
    [ContextMenu("ResetGrid")]
    void ResetGrid()
    {
        if (flatCells == null || flatCells.Count == 0) return;

            foreach (Cell cell in flatCells)
            {
                DestroyImmediate(cell.gameObject);
            }
            flatCells = new List<Cell>();


    }
}
