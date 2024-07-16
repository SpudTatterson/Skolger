using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpilePlacer : MonoBehaviour
{
    public static StockpilePlacer instance {get; private set;}
    public List<Cell> cells;

    Cell firstCell = null;
    Cell previousCell;
    GameObject tempGrid;
    bool makingStockpile = false;
    bool shrinking = false;
    bool growing = false;
    bool inUse = false;
    Stockpile selectedStockpile;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.Log("More then 1 StockpilePlacer Exists");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StopMakingStockpile();
        }

        if (!inUse) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out RaycastHit hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            firstCell = GridManager.instance.GetCellFromPosition(hit.point);
        }
        else if (Input.GetKey(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            Cell currentCell = GridManager.instance.GetCellFromPosition(hit.point);

            if (previousCell == null || currentCell != previousCell)
            {
                var (size, cornerCell) = GridObject.GetGridBoxFrom2Cells(firstCell, currentCell);
                Destroy(tempGrid);
                Vector3 cornerPos = cornerCell.position - new Vector3(0.5f, -0.01f, 0.5f);
                tempGrid = MeshUtility.CreateGridMesh(size.x, size.y, cornerPos, "StockpileTempVisual", MaterialManager.instance.stockpileMaterial);
            }

            previousCell = currentCell;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, 500f, LayerManager.instance.GroundLayerMask)
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
    }

    public void ShrinkStockpile(Stockpile stockpile)
    {
        shrinking = true;
        growing = false;
        inUse = true;

        selectedStockpile = stockpile;

        SelectionManager.instance.isSelecting = false;
    }
    public void GrowStockpile(Stockpile stockpile)
    {
        growing = true;
        shrinking = false;
        inUse = true;

        selectedStockpile = stockpile;

        SelectionManager.instance.isSelecting = false;
    }

    public void StartMakingStockPile()
    {
        inUse = true;
        makingStockpile = true;
        SelectionManager.instance.isSelecting = false;
    }
    public void StopMakingStockpile()
    {
        SelectionManager.instance.isSelecting = true;
        Destroy(tempGrid);
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
