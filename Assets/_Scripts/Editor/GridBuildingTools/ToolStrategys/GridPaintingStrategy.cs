using System.Collections.Generic;
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