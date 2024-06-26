using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GridObject : MonoBehaviour, ISerializationCallbackReceiver
{
    [Header("Settings")]
    [SerializeField, ReadOnly] int height;
    [SerializeField, ReadOnly] int width;
    [SerializeField, ReadOnly] float cellSize;
    [SerializeField, ReadOnly] float cellHeight;
    [SerializeField, ReadOnly] Material material;
    int groundLayer = 7;
    bool startEmpty;

    [Header("Cells")]
    public Cell[,] cells;
    [SerializeField] List<Cell> flatCells = new List<Cell>();
    List<GameObject> visualGridChunks = new List<GameObject>();

    public static GridObject MakeInstance(WorldSettings worldSettings, bool startEmpty, Vector3 position, Transform parent, string gridName)
    {
        GameObject gridGO = new GameObject(gridName);
        gridGO.transform.position = position;
        gridGO.transform.parent = parent;

        GridObject gridObject = gridGO.AddComponent<GridObject>();
        gridObject.Init(worldSettings, startEmpty);

        NavMeshSurface navMeshSurface = gridGO.AddComponent<NavMeshSurface>();
        navMeshSurface.collectObjects = CollectObjects.Children;
        navMeshSurface.BuildNavMesh();

        return gridObject;
    }
    public void Init(WorldSettings worldSettings, bool startEmpty)
    {
        this.height = worldSettings.gridXSize;
        this.width = worldSettings.gridYSize;
        this.cellSize = worldSettings.cellSize;
        this.cellHeight = worldSettings.cellHeight;
        this.material = worldSettings.material;
        this.groundLayer = worldSettings.groundLayer;
        this.startEmpty = startEmpty;

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

    [Button, ContextMenu("ResetGrid")]
    public void ResetGrid()
    {
        InitializeCells();
        if (visualGridChunks.Count == 0)
        {
            List<MeshFilter> meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
            foreach (MeshFilter filter in meshFilters)
            {
                visualGridChunks.Add(filter.gameObject);
            }
        }
        foreach (GameObject chunk in visualGridChunks)
        {
            DestroyImmediate(chunk);
        }
        visualGridChunks.Clear();
        Debug.Log("Old Grid Deleted");
    }
    [Button, ContextMenu("UpdatedGridVisual")]
    public void UpdateVisualGrid()
    {
        // Clear existing visual grid
        visualGridChunks.ForEach(chunk => DestroyImmediate(chunk));
        visualGridChunks.Clear();

        // Recreate visual grid based on cell visibility
        CreateVisualGrid();

        // update nav mesh
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    [Button, ContextMenu("GenerateGrid")] //allows calling function from editor 
    public void GenerateGrid()
    {
        ResetGrid();

        flatCells = new List<Cell>();
        EditorUtility.SetDirty(this);

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
        Cell cell = new Cell(x, y, !startEmpty, GetWorldPosition(x, y), this);
        cells[x, y] = cell;
        flatCells.Add(cell);
    }

    void CreateVisualGrid()
    {
        int chunkSize = 50; // Adjust the chunk size to ensure you stay within the vertex limit

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
        GameObject gridVisual = new GameObject("GridChunk_" + startX + "_" + startY)
        {
            layer = groundLayer,
        };
        gridVisual.transform.SetParent(transform);
        MeshFilter meshFilter = gridVisual.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gridVisual.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        int numberOfCells = chunkWidth * chunkHeight;
        int verticesPerCell = 20; // 5 faces, 4 vertices each
        int trianglesPerCell = 30; // 5 faces, 6 indices each

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[numberOfCells * verticesPerCell];
        int[] triangles = new int[numberOfCells * trianglesPerCell];
        Vector2[] uv = new Vector2[numberOfCells * verticesPerCell];

        int vertIndex = 0;
        int triIndex = 0;

        bool meshEmpty = true;
        bool hasInvisibleCell = false;

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                Cell cell = cells[startX + x, startY + y];
                if (cell == null || !cell.isVisible)
                {
                    hasInvisibleCell = true;
                    continue;
                }

                meshEmpty = false;

                Vector3 basePosition = new Vector3((startX + x) * cellSize, 0, (startY + y) * cellSize);

                // Define vertices for the cube (excluding the bottom face)
                Vector3[] cubeVertices = new Vector3[]
                {
                // Top face
                new Vector3(0, 0, 0),
                new Vector3(0, 0, cellSize),
                new Vector3(cellSize, 0, cellSize),
                new Vector3(cellSize, 0, 0),
                // Front face
                new Vector3(0, 0, cellSize),
                new Vector3(cellSize, 0, cellSize),
                new Vector3(0, -cellHeight, cellSize),
                new Vector3(cellSize, -cellHeight, cellSize),
                // Back face
                new Vector3(0, 0, 0),
                new Vector3(cellSize, 0, 0),
                new Vector3(0, -cellHeight, 0),
                new Vector3(cellSize, -cellHeight, 0),
                // Left face
                new Vector3(0, 0, 0),
                new Vector3(0, 0, cellSize),
                new Vector3(0, -cellHeight, 0),
                new Vector3(0, -cellHeight, cellSize),
                // Right face
                new Vector3(cellSize, 0, 0),
                new Vector3(cellSize, 0, cellSize),
                new Vector3(cellSize, -cellHeight, 0),
                new Vector3(cellSize, -cellHeight, cellSize)
                };

                // Apply transform to vertices
                for (int i = 0; i < cubeVertices.Length; i++)
                {
                    vertices[vertIndex + i] = basePosition + cubeVertices[i] + transform.position;
                }

                // Define triangles for the cube (excluding the bottom face)
                int[] cubeTriangles = new int[]
                {
                // Top face
                0, 1, 2, 2, 3, 0,
                // Front face
                6, 5, 4, 5, 6, 7,
                // Back face
                8, 9, 10, 11, 10, 9,
                // Left face
                14, 13, 12, 13, 14, 15,
                // Right face
                19, 16, 17, 16, 19, 18
                };

                // Assign triangles with offset
                for (int i = 0; i < cubeTriangles.Length; i++)
                {
                    triangles[triIndex + i] = vertIndex + cubeTriangles[i];
                }

                // Define UVs for the cube using texture atlas
                int textureIndex = (int)cell.cellType; // You'll need to define how to get the texture index for each cell
                Vector2[] cubeUVs = GetUVsForCellAtlas((int)cell.cellType);

                // Assign UVs with offset
                for (int i = 0; i < cubeUVs.Length; i++)
                {
                    uv[vertIndex + i] = cubeUVs[i];
                }

                // Increment indices for the next cube
                vertIndex += cubeVertices.Length;
                triIndex += cubeTriangles.Length;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.name = gridVisual.name;

        if (!meshEmpty)
        {
            meshFilter.mesh = mesh;

            MeshCollider mc = gridVisual.AddComponent<MeshCollider>();
            if(!hasInvisibleCell) mc.convex = true;
        }

        visualGridChunks.Add(gridVisual);
    }
    Vector2[] GetUVsForCellAtlas(int textureIndex)
    {
        int atlasSize = 2; // Assuming a 4x4 texture atlas
        float uvSize = 1.0f / atlasSize;

        int xIndex = textureIndex % atlasSize;
        int yIndex = atlasSize - 1 - (textureIndex / atlasSize);

        float u = xIndex * uvSize;
        float v = yIndex * uvSize;

        // Define UV mapping for a cell in the texture atlas
        return new Vector2[]
        {
        // Top face
        new Vector2(u, v),
        new Vector2(u, v + uvSize),
        new Vector2(u + uvSize, v + uvSize),
        new Vector2(u + uvSize, v),
        // Front face
        new Vector2(u, v),
        new Vector2(u + uvSize, v),
        new Vector2(u, v + uvSize),
        new Vector2(u + uvSize, v + uvSize),
        // Back face
        new Vector2(u, v),
        new Vector2(u + uvSize, v),
        new Vector2(u, v + uvSize),
        new Vector2(u + uvSize, v + uvSize),
        // Left face
        new Vector2(u, v),
        new Vector2(u + uvSize, v),
        new Vector2(u, v + uvSize),
        new Vector2(u + uvSize, v + uvSize),
        // Right face
        new Vector2(u, v),
        new Vector2(u + uvSize, v),
        new Vector2(u, v + uvSize),
        new Vector2(u + uvSize, v + uvSize)
        };
    }
    [Button]
    public void SaveAllChunkMeshesToFile()
    {
        List<MeshFilter> chunksMF = GetComponentsInChildren<MeshFilter>().ToList();
        foreach (var mf in chunksMF)
        {
            if (mf.sharedMesh == null) continue;
            Mesh mesh = SaveMeshToFile(mf.gameObject, mf.sharedMesh);
            mf.sharedMesh = mesh;
            mf.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }
    Mesh SaveMeshToFile(GameObject visualGO, Mesh mesh)
    {
        if (mesh != null)
        {
            // Get the active scene's path and ensure the directory exists
            string scenePath = SceneManager.GetActiveScene().path.Replace(".unity", "");
            string directoryPath = $"{scenePath}/Meshes/{gameObject.name}";

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            // Save the mesh to the file
            string path = $"{directoryPath}/{visualGO.name}.asset";

            // Check if the asset already exists
            Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existingMesh != null)
            {
                if (existingMesh != mesh)
                {
                    // Assign new mesh data to existing mesh asset
                    existingMesh.Clear();
                    existingMesh.vertices = mesh.vertices;
                    existingMesh.triangles = mesh.triangles;
                    existingMesh.uv = mesh.uv;
                    existingMesh.normals = mesh.normals;
                    existingMesh.RecalculateBounds();
                    AssetDatabase.SaveAssets();
                    Debug.Log("Updated mesh at " + path);
                }
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
                Debug.Log("Saved mesh to " + path);
                existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            }

            return existingMesh;
        }
        AssetDatabase.SaveAssets();
        return null;
    }

    #endregion

    #region Public Methods

    public Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position + new Vector3(x * cellSize + cellSize / 2, 0, y * cellSize + cellSize / 2);
    }

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

    public void ChangeCellsVisibility(List<Cell> cells, bool visible)
    {
        foreach (Cell cell in cells)
        {
            this.cells[cell.x, cell.y].isVisible = visible;
        }
        EditorUtility.SetDirty(this);
    }
    public void ChangeCellsType(List<Cell> cells, CellType type)
    {
        foreach (Cell cell in cells)
        {
            this.cells[cell.x, cell.y].cellType = type;
        }
        EditorUtility.SetDirty(this);
    }
    public void ResetCellUse()
    {
        foreach (Cell cell in cells)
        {
            cell.SetUseAndWalkable(false, true);
        }
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

    public static (Vector2Int size, Cell cornerCell) GetGridBoxFrom2Cells(Cell cell1, Cell cell2)
    {
        int xMin = Mathf.Min(cell1.x, cell2.x);
        int yMin = Mathf.Min(cell1.y, cell2.y);
        int xMax = Mathf.Max(cell1.x, cell2.x);
        int yMax = Mathf.Max(cell1.y, cell2.y);

        Vector2Int size = new Vector2Int(xMax - xMin + 1, yMax - yMin + 1);
        Cell cornerCell = cell1.grid.GetCellFromIndex(xMin, yMin);

        return (size, cornerCell);
    }

    public static (Vector2Int size, Cell cornerCell) GetGridLineFrom2Cells(Cell cell1, Cell cell2)
    {
        int xMin = Mathf.Min(cell1.x, cell2.x);
        int yMin = Mathf.Min(cell1.y, cell2.y);
        int xMax = Mathf.Max(cell1.x, cell2.x);
        int yMax = Mathf.Max(cell1.y, cell2.y);

        int width = xMax - xMin + 1;
        int height = yMax - yMin + 1;

        if (width >= height) // Horizontal line is longer or equal
        {
            Vector2Int size = new Vector2Int(width, 1);
            Cell cornerCell = cell1.grid.GetCellFromIndex(xMin, cell1.y);
            return (size, cornerCell);
        }
        else // Vertical line is longer
        {
            Vector2Int size = new Vector2Int(1, height);
            Cell cornerCell = cell1.grid.GetCellFromIndex(cell1.x, yMin);
            return (size, cornerCell);
        }
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

    [ContextMenu("TestCells"), Button]
    void TestCells()
    {
        foreach (Cell cell in cells)
        {
            Debug.Log(cell.grid == null);
        }
    }
}
