using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        GridBuilderTool.instance.Repaint();
    }

    public void DecreaseBrushSize()
    {
        brushSize = Mathf.Max(1f, brushSize - 1f); // Prevent brush size from going below 1
        GridBuilderTool.instance.Repaint();
    }

    #endregion
    
}