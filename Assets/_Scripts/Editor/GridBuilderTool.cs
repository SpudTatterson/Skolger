using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class GridBuilderTool : EditorWindow, IBrushTool
{

    //General Vars
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;
    static GridBuilderTool instance;

    string[] tools = { "World Sculpting", "Building Placer" };
    ActiveTool activeTool;
    enum ActiveTool
    {
        WorldSculpting,
        BuildingPlacer,
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

        if (activeTool == ActiveTool.BuildingPlacer)
        {
            DrawBuildingPlacerToolGUI();
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (gridManager == null) return;
        if (activeTool == ActiveTool.WorldSculpting)
            SculptWorld();
        else if (activeTool == ActiveTool.BuildingPlacer)
            BuildingPlacer();
    }

    #region BuildingPlacer

    void DrawBuildingPlacerToolGUI()
    {
        DrawDragAndDropArea();

        if (buildingDatas.Count > 0)
        {
            if (GUILayout.Button("Start Placing"))
            {
                BrushToolManager.DisableAllBrushTools();
                isPlacing = !isPlacing;
            }
            if(GUILayout.Button("Recalculate Cell Usage"))
            {
                gridManager.RecalculateCellUsage();
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

    void DrawDragAndDropArea()
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
                                Debug.Log("test");
                            }
                        }
                    }
                }
                Event.current.Use();
                break;
        }
    }

    void BuildingPlacer()
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

                if (!cellFree) return;

                if ((e.type == EventType.MouseDown || e.type == EventType.MouseDown) && e.button == 0)
                {
                    PlaceBuilding(cell, buildingDatas[selectedBuilding]);
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

        BuildingObject placed = BuildingObject.MakeInstance(buildingData, cell.position, cells);
        Undo.RegisterCreatedObjectUndo(placed.gameObject, $"Created {buildingDatas[selectedBuilding].name}");
        Undo.RegisterCompleteObjectUndo(cell.grid, $"Created {buildingDatas[selectedBuilding].name}");
    }

    #endregion

    #region WorldSculpting

    void DrawWorldSculptingToolGUI()
    {
        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        if (GUILayout.Button("Start Painting"))
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

    void SculptWorld()
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

    private void SelectCells(Vector3 center)
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

    private void ApplyVisibilityChanges()
    {
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

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        if (instance == null) instance = GetWindow<GridBuilderTool>();
        BrushToolManager.RegisterTool(instance);
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        BrushToolManager.UnregisterTool(instance);
    }
}