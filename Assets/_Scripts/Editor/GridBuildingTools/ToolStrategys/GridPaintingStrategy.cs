using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridPaintingStrategy : IGridToolStrategy, IBrushTool
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;

    public bool isPainting { get; private set; }

    CellType cellType;
    float brushSize = 1f;

    List<Cell> selectedCells = new List<Cell>();

    public GridPaintingStrategy(GridManager gridManager, LayerManager layerManager)
    {
        this.gridManager = gridManager;
        this.layerManager = layerManager;
    }
    public void OnGUI()
    {
        GUILayout.Label("Grid Painting Tool", EditorStyles.boldLabel);
        if (GUILayout.Button("Save All Grid Meshes"))
        {
            gridManager.SaveAllGridMeshesToFile();
        }

        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);

        cellType = (CellType)EditorGUILayout.EnumPopup(cellType);

        if (GUILayout.Button("Start Painting"))
        {
            StartTool();
        }
    }

    public void OnSceneGUI()
    {
        if (gridManager == null) return;

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

        Vector3 flattenedCenter = VectorUtility.FlattenVector(center);
        int gridXSize = gridManager.worldSettings.gridXSize;
        int gridYSize = gridManager.worldSettings.gridYSize;
        float cellSize = gridManager.worldSettings.cellSize; // Assuming you have this in your world settings

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

    void ApplyTextureChanges()
    {
        activeGridObject.ChangeCellsType(selectedCells, cellType);

        activeGridObject.UpdateVisualGrid();
        selectedCells.Clear();
        activeGridObject = null;
    }

    public void StartTool()
    {
        BrushToolManager.DisableAllBrushTools();
        isPainting = true;
    }

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