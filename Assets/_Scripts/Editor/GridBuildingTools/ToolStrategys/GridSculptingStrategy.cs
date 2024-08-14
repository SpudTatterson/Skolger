using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class GridSculptingStrategy : IGridToolStrategy, IBrushTool
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;

    public bool isPainting { get; private set; }
    bool isRaising;
    bool isLowering;
    float brushSize = 1f;
    private List<Cell> selectedCells = new List<Cell>();

    public GridSculptingStrategy(GridManager gridManager, LayerManager layerManager)
    {
        this.gridManager = gridManager;
        this.layerManager = layerManager;
    }

    #region IGridTool
    public void OnGUI()
    {
        if (GUILayout.Button("Save All Grid Meshes"))
        {
            gridManager.SaveAllGridMeshesToFile();
        }

        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        if (GUILayout.Button("Start Sculpting"))
        {
            StartTool();
        }
        GUILayout.Label("To lower hold Alt");
    }

    public void OnSceneGUI()
    {
        if (isPainting)
        {
            Event e = Event.current;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (e.type == EventType.MouseDown && e.button == 2)
            {
                isPainting = false;
                e.Use();
                return;
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

    public void StartTool()
    {
        BrushToolManager.DisableAllBrushTools();
        isPainting = true;
        isLowering = false;
        isRaising = true;
    }

    void SelectCells(Vector3 center)
    {
        if (activeGridObject == null)
        {
            Vector3 adjustedCenter = isRaising ? center + new Vector3(0, gridManager.worldSettings.cellHeight, 0) : center;
            activeGridObject = gridManager.GetGridFromPosition(adjustedCenter);
        }

        Vector3 flattenedCenter = VectorUtility.FlattenVector(center);
        int gridXSize = gridManager.worldSettings.gridXSize;
        int gridYSize = gridManager.worldSettings.gridYSize;
        float cellSize = gridManager.worldSettings.cellSize;

        // Calculate the range in indices to limit the loops
        int minX = Mathf.Max(0, Mathf.FloorToInt((flattenedCenter.x - brushSize) / cellSize));
        int maxX = Mathf.Min(gridXSize - 1, Mathf.CeilToInt((flattenedCenter.x + brushSize) / cellSize));
        int minY = Mathf.Max(0, Mathf.FloorToInt((flattenedCenter.z - brushSize) / cellSize));
        int maxY = Mathf.Min(gridYSize - 1, Mathf.CeilToInt((flattenedCenter.z + brushSize) / cellSize));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Cell cell = activeGridObject.GetCellFromIndex(x, y);
                if (cell != null)
                {
                    Vector3 flattenedCellPosition = VectorUtility.FlattenVector(cell.position);
                    float distance = Vector3.Distance(flattenedCellPosition, flattenedCenter);
                    if (distance <= brushSize)
                    {
                        selectedCells.Add(cell);
                    }
                }
            }
        }
        selectedCells.Distinct();
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
        GridBuilderTool.instance.Repaint();
    }

    public void DecreaseBrushSize()
    {
        brushSize = Mathf.Max(1f, brushSize - 1f); // Prevent brush size from going below 1
        GridBuilderTool.instance.Repaint();
    }

    #endregion

}