
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GridManager : MonoBehaviour, ISerializationCallbackReceiver
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

    private void ConvertFlatArrayTo2D()
    {
        if (flatCells != null && flatCells.Count > 0)
        {
            cells = new Cell[width, height];
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
        if (cells != null)
        {
            foreach (Cell cell in cells)
            {
                if (cell != null)
                    DestroyImmediate(cell.gameObject); // destroy all old cells when generating new grid this is not reversible  
            }
        }
        cells = new Cell[width, height];
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
        GameObject cellVisual = Instantiate(cellPrefabs[randomCellPrefabIndex], GetWorldPosition(x, y), Quaternion.identity, this.transform);
        cellVisual.name = x + "/" + y + " " + "Cell";
        Cell cell = cellVisual.GetComponent<Cell>();
        cells[x, y] = cell;
        flatCells.Add(cell);

    }


    void UpdateCellInfo(int x, int y)
    {
        cells[x, y].SetXY(x, y);
        cells[x, y].SetGridManager(this);
    }

    //[ContextMenu("UpdateGrid")] dont use this for now
    void UpdateGrid()
    {
        if (cells == null) ConvertFlatArrayTo2D();
        Cell[,] tempCopy = cells.Clone() as Cell[,]; // create backup of grid

        cells = new Cell[width, height]; // expand/shrink array


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
    public List<Vector2Int> GetGridIndexes(Vector2Int initialIndex, int width, int height)
    {
        List<Vector2Int> indexes = new List<Vector2Int>();

        // Iterate through the grid starting from initialIndex up to calculated max bounds
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Add the coordinates to the list
                indexes.Add(initialIndex + new Vector2Int(x, y));
            }
        }
        return indexes;
    }
    public List<Cell> GetGrids(Vector2Int initialIndex, int width, int height)
    {
        List<Vector2Int> indexes = GetGridIndexes(initialIndex, width, height);
        List<Cell> cells = new List<Cell>();

        foreach (Vector2Int i in indexes)
        {
            cells.Add(this.cells[i.x, i.y]);
        }
        return cells;
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
        if(cells == null) return;
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
    [ContextMenu("ReadCells")]
    void ReadCells()
    {
        foreach (Cell cell in cells)
        {
            Debug.Log(cell.name);
        }
    }
}
