using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingPlacerStrategy : IGridToolStrategy
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;

    int selectedBuilding = 0;
    List<Texture> buildingIcons = new List<Texture>();
    List<BuildingData> buildingDatas = new List<BuildingData>();
    Vector2 scrollPos;
    bool isPlacing = false;
    float initialMouseDownTime = 0;
    float timeToInitDrag = 0.1f;
    Cell firstCell;
    Cell cornerCell;
    Cell lastCell;

    public BuildingPlacerStrategy(GridManager gridManager, LayerManager layerManager)
    {
        this.gridManager = gridManager;
        this.layerManager = layerManager;
    }

    public void OnGUI()
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
            scrollPos = GUILayout.BeginScrollView(scrollPos);

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

    public void OnSceneGUI()
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

    public void StartTool()
    {
        throw new System.NotImplementedException();
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
}