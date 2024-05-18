using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileTest : MonoBehaviour
{
    public List<Cell> cells;
    [SerializeField] Vector2Int size = new Vector2Int(1, 1);

    bool stockpileMade = false;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500f, LayerManager.instance.GroundLayerMask) && Input.GetKeyDown(KeyCode.Mouse0) && stockpileMade == false)
        {
            GridManager grid = hit.transform.GetComponentInParent<GridManager>();
            Cell cell = grid.GetCellFromPosition(hit.point);
            bool cellsExist = grid.TryGetCells(new Vector2Int(cell.x, cell.y), size.x, size.y, out cells);
            bool cellsFree = true;
            foreach (Cell c in cells)
            {
                cellsFree = c.IsFreeForBuilding();
                if (cellsFree == false) break;
            }
            if (cellsExist && cellsFree)
            {
                GameObject stockpileGO = new GameObject("Stockpile");
                Stockpile stockpile = stockpileGO.AddComponent<Stockpile>();
                stockpile.Initialize(size.x, size.y, cells, cell.position, 1);
                foreach (Cell c in cells)
                {
                    c.inUse = true;
                    c.Walkable = true;
                }
                stockpileMade = true;
            }
        }
        RaycastHit itemHit;
        if (Physics.Raycast(ray, out itemHit, 100, LayerManager.instance.ItemLayerMask) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ItemObject item = itemHit.transform.GetComponent<ItemObject>();
            Stockpile stockpile = InventoryManager.instance.stockpiles[0];
            Cell cell;
            if (stockpile.GetEmptyCell(out cell))
            {
                Debug.Log("yes");
                stockpile.AddItem(cell, item);
                // Destroy(item.visualGO); // if we destroy it we lose data we need to either move it or figure out something else
                Stockpile itemStockpile;
                if (item.CheckIfInStockpile(out itemStockpile))
                {
                    itemStockpile.TakeItem(cell);
                }
                else
                { 
                    Destroy(itemHit.transform.gameObject);
                }
            }
            else
            {
                Debug.Log("No empty spots found in stockpile");
            }

        }
    }
}
