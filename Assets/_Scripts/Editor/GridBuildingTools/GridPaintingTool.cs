using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using NaughtyAttributes.Editor;
using NaughtyAttributes;

public class GridPaintingTool : EditorWindow, IBrushTool
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;

    public bool isPainting { get; private set; }

    CellType cellType;
    float brushSize = 1f;

    List<Cell> selectedCells = new List<Cell>();

    private static GridPaintingTool instance;

    [MenuItem("Tools/Grid/Grid Painting Tool")]
    public static void ShowWindow()
    {
        instance = GetWindow<GridPaintingTool>("Grid Painting Tool");
    }
    void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        layerManager = FindAnyObjectByType<LayerManager>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Painting Tool", EditorStyles.boldLabel);
        if(GUILayout.Button("Save All Grid Meshes"))
        {
            gridManager.SaveAllGridMeshesToFile();
        }

        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        cellType = (CellType)EditorGUILayout.EnumPopup(cellType);

        if (GUILayout.Button("Start Painting"))
        {
            StartPainting();
        }

    }

    [Shortcut("GridTools/StartGridPainting", KeyCode.D, ShortcutModifiers.Alt)]
    static void StartPaintingShortcut()
    {
        GridPaintingTool window = GetWindow<GridPaintingTool>();
        window.StartPainting();
    }

    #region IBrushTool

    void StartPainting()
    {
        BrushToolManager.DisableAllBrushTools();
        isPainting = true;
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

    void OnSceneGUI(SceneView sceneView)
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
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerManager.GroundLayerMask))
            {
                Vector3 hitPoint = hit.point;
                Handles.color = Color.blue;
                Handles.DrawWireDisc(hitPoint, Vector3.up, brushSize);

                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
                {
                    SelectCells(hitPoint);
                    e.Use();
                }
                if (e.type == EventType.MouseUp && e.button == 0)
                    ApplyTextureChanges();
            }

            SceneView.RepaintAll();
        }
    }

    void SelectCells(Vector3 center)
    {
        if (activeGridObject == null)
        {
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

    void ApplyTextureChanges()
    {
        activeGridObject.ChangeCellsType(selectedCells, cellType);

        activeGridObject.UpdateVisualGrid();
        selectedCells.Clear();
        activeGridObject = null;
    }

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