using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileTest : MonoBehaviour
{
    public List<Cell> cells;

    bool makingStockpile = false;
    Cell firstCell = null;
    Cell previousCell;
    GameObject tempGrid;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StopMakingStockpile();
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!makingStockpile) return;
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out RaycastHit hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            firstCell = GridManager.instance.GetCellFromPosition(hit.point);
            Debug.Log("started");
        }
        else if (Input.GetKey(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            Cell currentCell = GridManager.instance.GetCellFromPosition(hit.point);

            if (previousCell == null || currentCell != previousCell)
            {
                var (size, cornerCell) = GridObject.GetGridSizeFrom2Cells(firstCell, currentCell);
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
            var (size, cornerCell) = GridObject.GetGridSizeFrom2Cells(firstCell, cell);
            bool cellsExist = grid.TryGetCells(new Vector2Int(cornerCell.x, cornerCell.y), size.x, size.y, out cells);
            bool cellsFree = true;
            foreach (Cell c in cells)
            {
                cellsFree = c.IsFreeForBuilding();
                if (!cellsFree) break;
            }
            if (cellsExist && cellsFree)
            {
                GameObject stockpileGO = new GameObject("Stockpile");
                Stockpile stockpile = stockpileGO.AddComponent<Stockpile>();
                stockpile.Initialize(size.x, size.y, cells, cornerCell.position, 1);
                foreach (Cell c in cells)
                {
                    c.inUse = true;
                    c.Walkable = true;
                }
            }
            StopMakingStockpile();
        }
    }


    public void StartMakingStockPile()
    {
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
    }
}
