using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileTest : MonoBehaviour
{
    public List<Cell> cells;

    bool makingStockpile = false;
    Cell firstCell;
    Cell previousCell;
    GameObject tempGrid;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(!makingStockpile) return;
        if(Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out RaycastHit hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            firstCell = GridManager.instance.GetCellFromPosition(hit.point);
        }
        else if(Input.GetKey(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            Cell currentCell = GridManager.instance.GetCellFromPosition(hit.point);
            
            if(previousCell == null || currentCell != previousCell)
            {
                Vector2Int size = GridObject.GetGridSizeFrom2Cells(firstCell, currentCell);
                Destroy(tempGrid);
                Vector3 cornerPos = firstCell.position - new Vector3(0.5f, -0.01f, 0.5f);
                tempGrid = MeshUtility.CreateGridMesh(size.x, size.y, cornerPos, "StockpileTempVisual", MaterialManager.instance.stockpileMaterial);
            }

            previousCell = currentCell;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, 500f, LayerManager.instance.GroundLayerMask))
        {
            Destroy(tempGrid);

            GridObject grid = hit.transform.GetComponentInParent<GridObject>();
            Cell cell = grid.GetCellFromPosition(hit.point);
            Vector2Int size = GridObject.GetGridSizeFrom2Cells(firstCell, cell);
            bool cellsExist = grid.TryGetCells(new Vector2Int(firstCell.x, firstCell.y), size.x, size.y, out cells);
            bool cellsFree = true;
            foreach (Cell c in cells)
            {
                cellsFree = c.IsFreeForBuilding();
                if (cellsFree == false) break;
            }
            if (cellsExist && cellsFree)
            {
                makingStockpile = false;
                GameObject stockpileGO = new GameObject("Stockpile");
                Stockpile stockpile = stockpileGO.AddComponent<Stockpile>();
                stockpile.Initialize(size.x, size.y, cells, firstCell.position, 1);
                foreach (Cell c in cells)
                {
                    c.inUse = true;
                    c.Walkable = true;
                }
            }
        }
    }

    public void StartMakingStockPile()
    {
        makingStockpile = true;
    }
}
