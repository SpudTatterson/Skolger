using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using Unity.VisualScripting;

public class GridBuilderTool : EditorWindow, IBrushTool
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;
    public bool isPainting { get; private set; }
    bool isRaising;
    bool isLowering;
    float brushSize = 1f;
    static GridBuilderTool instance;

    private List<Cell> selectedCells = new List<Cell>();

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

    private void OnGUI()
    {
        GUILayout.Label("Grid Builder Tool", EditorStyles.boldLabel);

        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        if (GUILayout.Button("Start Painting"))
        {
            StartPainting();
        }
        GUILayout.Label("To lower hold Alt");

    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (gridManager == null) return;

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

    #region IBrushTool

    [Shortcut("GridTools/StartGridBuilding", KeyCode.F, ShortcutModifiers.Alt)]
    static void StartPaintingShortcut()
    {
        GridBuilderTool window = GetWindow<GridBuilderTool>();
        window.StartPainting();
    }

    void StartPainting()
    {
        BrushToolManager.DisableAllBrushTools();
        isPainting = true;
        isLowering = false;
        isRaising = true;
    }
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