using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GridObject : MonoBehaviour, ISerializationCallbackReceiver
{
    [Header("Settings")]
    [SerializeField] int height;
    [SerializeField] int width;
    [SerializeField] float cellSize;
    [SerializeField] GameObject[] cellPrefabs;
    [SerializeField] Material material;
    [SerializeField] int groundLayer;

    [Header("Cells")]
    public Cell[,] cells;
    [SerializeField] List<Cell> flatCells = new List<Cell>();
    List<GameObject> visualGridChunks = new List<GameObject>();


    public GridObject(int height, int width, float cellSize, GameObject[] cellPrefabs, Material material)
    {
        this.height = height;
        this.width = width;
        this.cellSize = cellSize;
        this.cellPrefabs = cellPrefabs;
        this.material = material;

        GenerateGrid();
    }
    void Awake()
    {
        ConvertFlatArrayTo2D();
    }

    #region Grid Generation
    public void InitializeCells()
    {
        cells = new Cell[width, height];
    }

    [ContextMenu("ResetGrid")]
    void ResetGrid()
    {
        if (flatCells == null) return;
        InitializeCells();
        if (visualGridChunks.Count == 0)
        {
            Debug.LogWarning("No old grid Chunks found \n if old grid chunks exist please delete manually");
            return;
        }
        foreach (GameObject chunk in visualGridChunks)
        {
            DestroyImmediate(chunk);
        }
        visualGridChunks.Clear();
        Debug.Log("Old Grid Deleted");
    }

    [ContextMenu("GenerateGrid")] //allows calling function from editor 
    void GenerateGrid()
    {
        ResetGrid();

        flatCells = new List<Cell>();

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                GenerateCell(x, y);
            }
        }
        CreateVisualGrid();
        Debug.Log("Grid Created");
    }

    void GenerateCell(int x, int y)
    {
        Cell cell = new Cell(x, y, GetWorldPosition(x, y), this);
        cells[x, y] = cell;
        flatCells.Add(cell);
    }

    void CreateVisualGrid()
    {
        int chunkSize = 100; // Adjust the chunk size to ensure you stay within the vertex limit

        for (int chunkX = 0; chunkX < width; chunkX += chunkSize)
        {
            for (int chunkY = 0; chunkY < height; chunkY += chunkSize)
            {
                int currentChunkWidth = Mathf.Min(chunkSize, width - chunkX);
                int currentChunkHeight = Mathf.Min(chunkSize, height - chunkY);

                CreateGridChunk(chunkX, chunkY, currentChunkWidth, currentChunkHeight);
            }
        }
    }

    void CreateGridChunk(int startX, int startY, int chunkWidth, int chunkHeight)
    {
        GameObject gridVisual = new GameObject("GridChunk_" + startX + "_" + startY);
        gridVisual.layer = groundLayer;
        gridVisual.transform.SetParent(transform);
        MeshFilter meshFilter = gridVisual.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gridVisual.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[chunkWidth * chunkHeight * 4];
        int[] triangles = new int[chunkWidth * chunkHeight * 6];
        Vector2[] uv = new Vector2[chunkWidth * chunkHeight * 4];

        int vertIndex = 0;
        int triIndex = 0;

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                Vector3 basePosition = new Vector3((startX + x) * cellSize, 0, (startY + y) * cellSize);

                vertices[vertIndex + 0] = basePosition + new Vector3(0, 0, 0) + transform.position;
                vertices[vertIndex + 1] = basePosition + new Vector3(0, 0, cellSize) + transform.position;
                vertices[vertIndex + 2] = basePosition + new Vector3(cellSize, 0, cellSize) + transform.position;
                vertices[vertIndex + 3] = basePosition + new Vector3(cellSize, 0, 0) + transform.position;

                triangles[triIndex + 0] = vertIndex + 0;
                triangles[triIndex + 1] = vertIndex + 1;
                triangles[triIndex + 2] = vertIndex + 2;
                triangles[triIndex + 3] = vertIndex + 2;
                triangles[triIndex + 4] = vertIndex + 3;
                triangles[triIndex + 5] = vertIndex + 0;

                uv[vertIndex + 0] = new Vector2(0, 0);
                uv[vertIndex + 1] = new Vector2(0, 1);
                uv[vertIndex + 2] = new Vector2(1, 1);
                uv[vertIndex + 3] = new Vector2(1, 0);

                vertIndex += 4;
                triIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        gridVisual.AddComponent<MeshCollider>();
        visualGridChunks.Add(gridVisual);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position + new Vector3(x * cellSize + cellSize / 2, 0, y * cellSize + cellSize / 2);
    }

    #endregion

    #region Public Methods

    public Cell GetCellFromPosition(Vector3 position)
    {
        Vector3 localPosition = position - transform.position;
        int x = Mathf.FloorToInt(localPosition.x / cellSize);
        int y = Mathf.FloorToInt(localPosition.z / cellSize);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return cells[x, y];
        }
        return null; // or handle out-of-bounds case
    }

    public bool TryGetCellIndexes(Vector2Int initialIndex, int width, int height, out List<Vector2Int> indexes)
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
    public bool TryGetCells(Vector2Int initialIndex, int width, int height, out List<Cell> cells)
    {
        List<Vector2Int> indexes;
        cells = new List<Cell>();

        bool insideArray = TryGetCellIndexes(initialIndex, width, height, out indexes);
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
        if (x < 0 || y < 0 || x >= width || y >= height) // Use '>= width' and '>= height' to check boundaries
            return null;
        else
            return cells[x, y];
    }

    public static Vector2Int GetGridSizeFrom2Cells(Cell Cell1, Cell cell2)
    {
        if (Cell1 == cell2) return new Vector2Int(1, 1);
        
        int xSize = Mathf.Abs(Cell1.x - cell2.x);
        int ySize = Mathf.Abs(Cell1.y - cell2.y);

        return new Vector2Int(xSize + 1, ySize + 1);
    }

    #endregion

    #region serialization 
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
    void ConvertFlatArrayTo2D()
    {
        if (flatCells != null && flatCells.Count > 0)
        {
            InitializeCells();
            for (int i = 0; i < flatCells.Count; i++)
            {
                int x = i / height;
                int y = i % height;
                cells[x, y] = flatCells[i];
                //cells[x, y].SetXY(x, y);
            }
        }
    }

    #endregion

    [ContextMenu("TestCells")]
    void TestCells()
    {
        foreach (Cell cell in cells)
        {
            Debug.Log(cell.id);
        }
    }
}
