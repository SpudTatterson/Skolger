using System.Collections.Generic;
using Sirenix.Utilities;
using SpudsUtility;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class ItemSpreaderStrategy : IGridToolStrategy, IBrushTool
{
    GridManager gridManager;
    LayerManager layerManager;
    GridObject activeGridObject;

    List<ItemData> itemDatas = new List<ItemData>();
    List<Texture2D> itemIcons = new List<Texture2D>();
    int selectedItem = 0;
    float coveragePercentage = 20;
    int amountToDisperse = 100;
    int minSplitAmount = 50;
    Vector2 scrollPos;

    public bool isPainting { get; private set; }
    float brushSize = 1;

    public ItemSpreaderStrategy(GridManager gridManager, LayerManager layerManager)
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

        UtilityEditor.DrawDragAndDropArea<ItemData>(AddItem, "Drag and Drop ItemData here");

        if (itemDatas.Count > 0)
        {
            if (GUILayout.Button("Start Placing"))
            {
                BrushToolManager.DisableAllBrushTools();
                isPainting = !isPainting;
            }
            if (GUILayout.Button("Remove Selected"))
            {
                itemIcons.RemoveAt(selectedItem);
                itemDatas.RemoveAt(selectedItem);
            }

            brushSize = EditorGUILayout.FloatField("Radius", brushSize);
            coveragePercentage = EditorGUILayout.Slider("Coverage", coveragePercentage, 1, 100);
            amountToDisperse = EditorGUILayout.IntField("Amount To Disperse", amountToDisperse);
            minSplitAmount = EditorGUILayout.IntField("Minimum Split Amount", minSplitAmount);

            // Begin the scroll view
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            // Define the fixed size for each element in the selection grid
            int numberOfColumns = 4;
            float buttonWidth = 51.2f;
            float buttonHeight = 51.2f;

            // Create a grid with fixed size buttons
            selectedItem = GUILayout.SelectionGrid(selectedItem, itemIcons.ToArray(), numberOfColumns,
                GUILayout.Width(buttonWidth * numberOfColumns),
                GUILayout.Height(buttonHeight * Mathf.Ceil(itemIcons.Count / (float)numberOfColumns)));

            // End the scroll view
            GUILayout.EndScrollView();
        }
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

    public void StartTool()
    {
        throw new System.NotImplementedException();
    }

    void AddItem(object draggedObject)
    {
        if (draggedObject is ItemData itemData)
        {
            if (!itemDatas.Contains(itemData))
            {
                itemDatas.Add(itemData);
                itemIcons.Add(itemData.icon.texture);
            }
        }
    }

    void Place(List<Cell> cells)
    {
        if (cells.Count == 0) return;
        Undo.RegisterCompleteObjectUndo(cells[0].grid, $"Created {itemDatas[selectedItem].name}");
        foreach (Cell cell in cells)
        {
            if (amountToDisperse <= 0) break;
            int amount = GetRandomAmount();
            ItemObject item = ItemObject.MakeInstance(itemDatas[selectedItem], amount, cell.position, false);

            Undo.RegisterCreatedObjectUndo(item, $"Created {itemDatas[selectedItem].name}");
        }

    }

    int GetRandomAmount()
    {
        int amount = Random.Range(minSplitAmount, itemDatas[selectedItem].stackSize + 1);
        amount = Mathf.Clamp(amount, 0, amountToDisperse);
        amountToDisperse -= amount;
        return amount;
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