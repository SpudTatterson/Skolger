using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StockpilePlacer : MonoSingleton<StockpilePlacer>
{
    public List<Cell> cells;

    Cell firstCell = null;
    Cell previousCell;
    GameObject tempGrid;
    bool makingStockpile = false;
    bool shrinking = false;
    bool growing = false;
    bool inUse = false;
    Stockpile selectedStockpile;
    GameObject tempCellVisual;

    public UnityEvent onStockpilePlaced;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
        {
            StopMakingStockpile();
        }

        if (!inUse || EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, LayerManager.Instance.GroundLayerMask))
        {
            Cell currentCell = GridManager.Instance.GetCellFromPosition(hit.point);
            if (currentCell != firstCell)
            {
                if (tempCellVisual == null)
                {
                    List<Cell> currentCellList = new List<Cell> { currentCell };
                    tempCellVisual = MeshUtility.CreateGridMesh(currentCellList, "tempCell", MaterialManager.Instance.materials.stockpileMaterial);
                }
                else
                {
                    tempCellVisual.SetActive(true);
                    tempCellVisual.transform.position = currentCell.position;
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                firstCell = currentCell;
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                if (previousCell == null || currentCell != previousCell)
                {
                    var (size, cornerCell) = GridObject.GetGridBoxFrom2Cells(firstCell, currentCell);
                    Destroy(tempGrid);
                    Vector3 cornerPos = cornerCell.position - new Vector3(0.5f, -0.01f, 0.5f);
                    tempGrid = MeshUtility.CreateGridMesh(size.x, size.y, cornerPos, "StockpileTempVisual", MaterialManager.Instance.materials.stockpileMaterial);
                }

                previousCell = currentCell;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0)
                && firstCell?.grid != null)
            {
                GridObject grid = hit.transform.GetComponentInParent<GridObject>();
                Cell cell = grid.GetCellFromPosition(hit.point);
                var (size, cornerCell) = GridObject.GetGridBoxFrom2Cells(firstCell, cell);
                if (!grid.TryGetCells(new Vector2Int(cornerCell.x, cornerCell.y), size.x, size.y, out List<Cell> allCells)) return; // if failed to get cells abort
                foreach (Cell c in allCells)
                {
                    if (c.IsFreeAndExists())
                        cells.Add(c);
                }

                if (makingStockpile) DoNewStockpileLogic(size, cornerCell);
                else if (shrinking) DoShrinkLogic(allCells);
                else if (growing) DoGrowLogic(allCells);

                StopMakingStockpile();
            }
        }
    }

    void DoShrinkLogic(List<Cell> cells)
    {
        selectedStockpile.ShrinkStockpile(cells);
    }
    void DoGrowLogic(List<Cell> cells)
    {
        selectedStockpile.GrowStockpile(cells);
    }
    void DoNewStockpileLogic(Vector2Int size, Cell cornerCell)
    {
        GameObject stockpileGO = new GameObject("Stockpile");
        Stockpile stockpile = stockpileGO.AddComponent<Stockpile>();
        stockpile.Initialize(size.x, size.y, cornerCell);
        foreach (Cell c in cells)
        {
            c.inUse = true;
            c.walkable = true;
        }

        onStockpilePlaced?.Invoke();
    }

    public void ShrinkStockpile(Stockpile stockpile)
    {
        shrinking = true;
        growing = false;
        inUse = true;

        selectedStockpile = stockpile;

        SelectionManager.Instance.isSelecting = false;
    }
    public void GrowStockpile(Stockpile stockpile)
    {
        growing = true;
        shrinking = false;
        inUse = true;

        selectedStockpile = stockpile;

        SelectionManager.Instance.isSelecting = false;
    }

    public void StartMakingStockPile()
    {
        inUse = true;
        makingStockpile = true;
        SelectionManager.Instance.isSelecting = false;
        BuildingPlacer.Instance.CancelPlacement();
    }
    public void StopMakingStockpile()
    {
        SelectionManager.Instance.isSelecting = true;
        Destroy(tempGrid);
        tempCellVisual?.SetActive(false);
        makingStockpile = false;
        firstCell = null;
        previousCell = null;
        cells.Clear();

        growing = false;
        shrinking = false;
        selectedStockpile = null;

        inUse = false;
    }
}
