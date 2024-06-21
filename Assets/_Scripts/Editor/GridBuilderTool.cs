using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GridBuilderTool : EditorWindow
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;
    int layerIndex;
    bool isRaising;
    bool isLowering;
    float brushSize = 1f;

    private List<Cell> selectedCells = new List<Cell>();

    [MenuItem("Tools/Grid Visibility Tool")]
    public static void ShowWindow()
    {
        GetWindow<GridBuilderTool>("Grid Visibility Tool");
    }
    void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        layerManager = FindAnyObjectByType<LayerManager>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Visibility Tool", EditorStyles.boldLabel);

        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        if (GUILayout.Button("Set Brush to Raise"))
        {
            isRaising = true;
            isLowering = false;
        }

        if (GUILayout.Button("Set Brush to Lower"))
        {
            isRaising = false;
            isLowering = true;
        }

        if (gridManager != null && selectedCells.Count > 0 && GUILayout.Button("Apply Visibility Changes"))
        {
            ApplyVisibilityChanges();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (gridManager == null) return;

        Event e = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (isRaising || isLowering)
        {
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
                if(e.type == EventType.MouseUp && e.button == 0)
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

        foreach (Cell cell in selectedCells)
        {
            cell.grid.ChangeCellVisibility(cell, visibility);
        }

        activeGridObject.UpdateVisualGrid();
        selectedCells.Clear();
        activeGridObject = null;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}