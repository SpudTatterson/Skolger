using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;

public class GridBuilderTool : EditorWindow, IBrushTool
{

    //General Vars
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;
    static GridBuilderTool instance;

    string[] tools = { "World Sculpting", "Building Placer", "Foliage Spreader" };
    ActiveTool activeTool;
    enum ActiveTool
    {
        WorldSculpting,
        BuildingPlacer,
        FoliageSpreader
    }


    //WorldSculpting
    public bool isPainting { get; private set; }
    bool isRaising;
    bool isLowering;
    float brushSize = 1f;
    private List<Cell> selectedCells = new List<Cell>();


    //BuildingPlacer

    int selectedBuilding = 0;
    List<Texture> buildingIcons = new List<Texture>();
    List<BuildingData> buildingDatas = new List<BuildingData>();
    Vector2 buildingScrollPos;
    bool isPlacing = false;
    float initialMouseDownTime = 0;
    float timeToInitDrag = 0.1f;
    Cell firstCell;
    Cell cornerCell;
    Cell lastCell;

    //Foliage Spreader

    List<GameObject> foliagePrefabs = new List<GameObject>();
    List<Texture2D> foliageIcons = new List<Texture2D>();
    int selectedFoliage = 0;
    float coveragePercentage = 20;


    [MenuItem("Tools/Grid/Grid Builder Tool")]
    public static void ShowWindow()
    {
        instance = GetWindow<GridBuilderTool>("Grid Builder Tool");
    }
    void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        layerManager = FindAnyObjectByType<LayerManager>();
    }

    void OnGUI()
    {
        GUILayout.Label("Grid Builder Tool", EditorStyles.boldLabel);

        activeTool = (ActiveTool)GUILayout.Toolbar((int)activeTool, tools);

        if (activeTool == ActiveTool.WorldSculpting)
        {
            DrawWorldSculptingToolGUI();
        }

        else if (activeTool == ActiveTool.BuildingPlacer)
        {
            DrawBuildingPlacerToolGUI();
        }

        else if (activeTool == ActiveTool.FoliageSpreader)
        {
            DrawFoliageSpreaderToolGUI();
        }

    }

    #region FoliageSpreader

    void DrawFoliageSpreaderToolGUI()
    {
        if (GUILayout.Button("Recalculate Cell Usage"))
        {
            gridManager.RecalculateCellUsage();
        }

        DrawFoliageSpreaderDragAndDropArea();

        if (foliagePrefabs.Count > 0)
        {
            if (GUILayout.Button("Start Placing"))
            {
                BrushToolManager.DisableAllBrushTools();
                isPainting = !isPainting;
            }
            if (GUILayout.Button("Remove Selected"))
            {
                foliageIcons.RemoveAt(selectedFoliage);
                foliagePrefabs.RemoveAt(selectedFoliage);
            }

            brushSize = EditorGUILayout.FloatField("Radius", brushSize);
            coveragePercentage = EditorGUILayout.Slider("Coverage", coveragePercentage, 1, 100);

            // Begin the scroll view
            buildingScrollPos = GUILayout.BeginScrollView(buildingScrollPos);

            // Define the fixed size for each element in the selection grid
            int numberOfColumns = 4;
            float buttonWidth = 51.2f;
            float buttonHeight = 51.2f;

            // Create a grid with fixed size buttons
            selectedFoliage = GUILayout.SelectionGrid(selectedFoliage, foliageIcons.ToArray(), numberOfColumns,
                GUILayout.Width(buttonWidth * numberOfColumns),
                GUILayout.Height(buttonHeight * Mathf.Ceil(foliageIcons.Count / (float)numberOfColumns)));

            // End the scroll view
            GUILayout.EndScrollView();
        }
    }

    void DrawFoliageSpreaderDragAndDropArea()
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and Drop GameObjects here");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (GameObject draggedObject in DragAndDrop.objectReferences)
                    {
                        if (!foliagePrefabs.Contains(draggedObject))
                        {
                            foliagePrefabs.Add(draggedObject);
                            Texture2D icon = AssetPreview.GetAssetPreview(draggedObject);
                            while (AssetPreview.IsLoadingAssetPreview(draggedObject.GetInstanceID()) == true || icon == null)
                            {
                                Debug.Log("Waiting for asset preview to load");
                                icon = AssetPreview.GetAssetPreview(draggedObject);
                            }
                            foliageIcons.Add(icon);
                        }
                    }
                }
                Event.current.Use();
                break;
        }
    }

    void FoliageSpreaderSceneGUI()
    {
        if (isPainting)
        {
            Event e = Event.current;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                isPainting = false;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerManager.GroundLayerMask))
            {
                Vector3 hitPoint = hit.point;
                Handles.color = Color.green;
                Handles.DrawWireDisc(hitPoint, Vector3.up, brushSize);

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    activeGridObject = gridManager.GetCellFromPosition(hitPoint).grid;
                    List<Cell> cells = GetCellsInCircle(hitPoint);
                    Place(cells);
                    e.Use();
                }
            }
        }
        SceneView.RepaintAll();
    }

    private void Place(List<Cell> cells)
    {
        if (cells.Count == 0) return;
        Undo.RegisterCompleteObjectUndo(cells[0].grid, $"Created {foliagePrefabs[selectedFoliage].name}");
        foreach (Cell cell in cells)
        {
            GameObject placedObject = PrefabUtility.InstantiatePrefab(foliagePrefabs[selectedFoliage]) as GameObject;
            placedObject.transform.position = cell.position;
            placedObject.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 180), 0));

            ICellOccupier occupier = placedObject.GetComponent<ICellOccupier>();
            occupier.GetOccupiedCells();
            occupier.OnOccupy();

            Undo.RegisterCreatedObjectUndo(placedObject, $"Created {foliagePrefabs[selectedFoliage].name}");
        }

    }

    List<Cell> GetCellsInCircle(Vector3 center)
    {
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < gridManager.worldSettings.gridXSize; x++)
        {
            for (int y = 0; y < gridManager.worldSettings.gridYSize; y++)
            {
                Cell cell = activeGridObject.GetCellFromIndex(x, y);
                if (cell != null)
                {
                    float distance = Vector3.Distance(VectorUtility.FlattenVector(cell.position), VectorUtility.FlattenVector(center));
                    if (distance <= brushSize)
                    {
                        if (!cells.Contains(cell) && cell.IsFreeAndExists())
                            cells.Add(cell);
                    }
                }
            }
        }
        return TakeCoverageIntoAccount(cells);
    }

    List<Cell> TakeCoverageIntoAccount(List<Cell> cells)
    {
        List<Cell> selectedCells = new List<Cell>();
        int cellsToSelect = Mathf.CeilToInt(cells.Count * (coveragePercentage / 100));
        for (int i = 0; i < cellsToSelect; i++)
        {
            int index = Random.Range(0, cells.Count);

            selectedCells.Add(cells[index]);
            cells.RemoveAt(index);
        }

        return selectedCells;
    }

    #endregion

    void OnSceneGUI(SceneView sceneView)
    {
        if (gridManager == null) return;
        if (activeTool == ActiveTool.WorldSculpting)
            SculptWorldSceneGUI();
        else if (activeTool == ActiveTool.BuildingPlacer)
            BuildingPlacerSceneGUI();
        else if (activeTool == ActiveTool.FoliageSpreader)
            FoliageSpreaderSceneGUI();
    }



    #region BuildingPlacer

    void DrawBuildingPlacerToolGUI()
    {
        if (GUILayout.Button("Recalculate Cell Usage"))
        {
            gridManager.RecalculateCellUsage();
        }

        DrawBuildingPlacerDragAndDropArea();

        if (buildingDatas.Count > 0)
        {
            if (GUILayout.Button("Start Placing"))
            {
                BrushToolManager.DisableAllBrushTools();
                isPlacing = !isPlacing;
            }

            // Begin the scroll view
            buildingScrollPos = GUILayout.BeginScrollView(buildingScrollPos);

            // Define the fixed size for each element in the selection grid
            int numberOfColumns = 4;
            float buttonWidth = 51.2f;
            float buttonHeight = 51.2f;

            // Create a grid with fixed size buttons
            selectedBuilding = GUILayout.SelectionGrid(selectedBuilding, buildingIcons.ToArray(), numberOfColumns,
                GUILayout.Width(buttonWidth * numberOfColumns),
                 GUILayout.Height(buttonHeight * Mathf.Ceil(buildingIcons.Count / (float)numberOfColumns)));

            // End the scroll view
            GUILayout.EndScrollView();
        }

    }

    void DrawBuildingPlacerDragAndDropArea()
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and Drop Building SO Here");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (ScriptableObject draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is BuildingData)
                        {
                            BuildingData buildingData = draggedObject as BuildingData;
                            if (!buildingDatas.Contains(buildingData))
                            {
                                buildingDatas.Add(buildingData);
                                buildingIcons.Add(buildingData.icon);
                            }
                        }
                    }
                }
                Event.current.Use();
                break;
        }
    }

    void BuildingPlacerSceneGUI()
    {
        if (isPlacing)
        {
            Event e = Event.current;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                isPlacing = false;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerManager.GroundLayerMask))
            {
                Cell cell = gridManager.GetCellFromPosition(hit.point);
                if (cell == null) return;
                Vector3 gridPoint = cell.position;
                activeGridObject = cell.grid;

                bool cellFree = cell.IsFreeAndExists();
                Handles.color = cellFree ? Color.green : Color.red;

                // Adjust the grid point to account for the building size
                Vector3 size = AdjustSizeAndGridPoint(ref gridPoint);
                Handles.DrawWireCube(gridPoint, size);
                if (cornerCell != null && lastCell != null)
                    Handles.DrawLine(cornerCell.position, lastCell.position);

                if (!cellFree) return;

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    // PlaceBuilding(cell, buildingDatas[selectedBuilding]);
                    initialMouseDownTime = (float)EditorApplication.timeSinceStartup;
                    firstCell = cell;
                    e.Use();
                }
                else if ((e.type == EventType.MouseDrag) && e.button == 0)
                {
                    (Vector2Int cellAmount, Cell cornerCell) = GridObject.GetGridLineFrom2Cells(firstCell, cell);

                    this.cornerCell = cornerCell;
                    if (cellAmount.x == 1)
                        lastCell = cornerCell.grid.GetCellFromIndex(cornerCell.x, cornerCell.y + cellAmount.y);
                    else
                        lastCell = cornerCell.grid.GetCellFromIndex(cornerCell.x + cellAmount.x, cornerCell.y);

                    e.Use();
                }
                else if ((e.type == EventType.MouseUp) && e.button == 0)
                {
                    if (initialMouseDownTime + timeToInitDrag >= (float)EditorApplication.timeSinceStartup)
                    {
                        PlaceBuilding(cell, buildingDatas[selectedBuilding]);
                    }
                    else
                    {
                        (Vector2Int cellAmount, Cell CornerCell) = GridObject.GetGridLineFrom2Cells(firstCell, cell);
                        activeGridObject.TryGetCells((Vector2Int)CornerCell, cellAmount.x, cellAmount.y, out List<Cell> cells);

                        foreach (Cell c in cells)
                        {
                            if (c.IsFreeAndExists())
                                PlaceBuilding(c, buildingDatas[selectedBuilding]);
                        }
                    }
                    firstCell = null;
                    lastCell = null;
                    cornerCell = null;
                    e.Use();
                }

            }

            SceneView.RepaintAll();
        }
    }

    Vector3 AdjustSizeAndGridPoint(ref Vector3 gridPoint)
    {
        float cellHeight = gridManager.worldSettings.cellHeight / 2f;
        float cellSize = gridManager.worldSettings.cellSize;
        float xSize = buildingDatas[selectedBuilding].xSize;
        float ySize = buildingDatas[selectedBuilding].ySize;

        gridPoint.x = gridPoint.x + (xSize / 2f) - cellSize * 0.5f;
        gridPoint.z = gridPoint.z + (ySize / 2f) - cellSize * 0.5f;
        gridPoint.y = activeGridObject.transform.position.y + cellHeight;


        Vector3 size = new Vector3(xSize, 3, ySize);
        return size;
    }

    void PlaceBuilding(Cell cell, BuildingData buildingData)
    {
        activeGridObject.TryGetCells((Vector2Int)cell, buildingData.xSize, buildingData.ySize, out List<Cell> cells);

        Undo.RegisterCompleteObjectUndo(cell.grid, $"Created {buildingDatas[selectedBuilding].name}");

        BuildingObject placed = BuildingObject.MakeInstance(buildingData, cell.position, cells);
        Undo.RegisterCreatedObjectUndo(placed.gameObject, $"Created {buildingDatas[selectedBuilding].name}");

    }

    #endregion

    #region WorldSculpting

    void DrawWorldSculptingToolGUI()
    {
        if (GUILayout.Button("Save All Grid Meshes"))
        {
            gridManager.SaveAllGridMeshesToFile();
        }

        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        if (GUILayout.Button("Start Sculpting"))
        {
            StartPainting();
        }
        GUILayout.Label("To lower hold Alt");
    }

    [Shortcut("GridTools/StartGridBuilding", KeyCode.F, ShortcutModifiers.Alt)]
    static void StartPaintingShortcut()
    {
        GridBuilderTool window = GetWindow<GridBuilderTool>();
        window.StartPainting();
    }

    void StartPainting()
    {
        BrushToolManager.DisableAllBrushTools();
        activeTool = ActiveTool.WorldSculpting;
        isPainting = true;
        isLowering = false;
        isRaising = true;

        isPlacing = false;
    }

    void SculptWorldSceneGUI()
    {
        if (isPainting)
        {
            Event e = Event.current;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                isPainting = false;
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftAlt)
            {
                isRaising = false;
                isLowering = true;
            }
            else if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftAlt)
            {
                isRaising = true;
                isLowering = false;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerManager.GroundLayerMask))
            {
                Vector3 hitPoint = hit.point;
                Handles.color = isLowering ? Color.red : Color.green;
                Handles.DrawWireDisc(hitPoint, Vector3.up, brushSize);

                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
                {
                    SelectCells(hitPoint);
                    e.Use();
                }
                if (e.type == EventType.MouseUp && e.button == 0)
                    ApplyVisibilityChanges();
            }

            SceneView.RepaintAll();
        }
    }

    void SelectCells(Vector3 center)
    {
        if (activeGridObject == null)
        {
            if (isRaising)
                activeGridObject = gridManager.GetGridFromPosition(center + new Vector3(0, gridManager.worldSettings.cellHeight, 0));
            else
                activeGridObject = gridManager.GetGridFromPosition(center);
        }

        for (int x = 0; x < gridManager.worldSettings.gridXSize; x++)
        {
            for (int y = 0; y < gridManager.worldSettings.gridYSize; y++)
            {
                Cell cell = activeGridObject.GetCellFromIndex(x, y);
                if (cell != null)
                {
                    float distance = Vector3.Distance(VectorUtility.FlattenVector(cell.position), VectorUtility.FlattenVector(center));
                    if (distance <= brushSize)
                    {
                        if (!selectedCells.Contains(cell))
                            selectedCells.Add(cell);
                    }
                }
            }
        }
    }

    void ApplyVisibilityChanges()
    {
        Undo.RegisterCompleteObjectUndo(activeGridObject, "World Sculpt");
        
        bool visibility = isRaising;

        activeGridObject.ChangeCellsVisibility(selectedCells, visibility);

        activeGridObject.UpdateVisualGrid();
        selectedCells.Clear();
        activeGridObject = null;
    }

    #endregion

    #region IBrushTool

    public void StopPainting()
    {
        isPainting = false;
    }

    public void IncreaseBrushSize()
    {
        brushSize += 1f;
        instance.Repaint();
    }

    public void DecreaseBrushSize()
    {
        brushSize = Mathf.Max(1f, brushSize - 1f); // Prevent brush size from going below 1
        instance.Repaint();
    }

    #endregion

    void OnBecameVisible()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        if (instance == null) instance = this;
        BrushToolManager.RegisterTool(instance);
    }

    void OnBecameInvisible()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        BrushToolManager.UnregisterTool(instance);
    }
}