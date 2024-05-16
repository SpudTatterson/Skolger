using System.Collections.Generic;
using UnityEngine;

public class TestingGridBuilding : MonoBehaviour
{
    public List<Cell> cells;
    public Building building;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            GridManager grid = hit.transform.GetComponentInParent<GridManager>();
            Cell cell = grid.GetCellFromPosition(hit.point);
            bool cellsExist = grid.TryGetCells(new Vector2Int(cell.x, cell.y), building.xSize, building.ySize, out cells);
            bool cellsFree = true;
            foreach(Cell c in cells)
            {
                cellsFree = c.IsFreeForBuilding();
                if(cellsFree == false) break;
            }
            if(cellsExist && cellsFree && InventoryManager.instance.HasMaterials(building.costs))
            {
                InventoryManager.instance.UseMaterials(building.costs);
                Debug.Log(InventoryManager.instance.CheckAmount(MaterialType.Wood));
                Instantiate(building.building, cell.position, Quaternion.identity);
                foreach (Cell c in cells)
                {
                    c.inUse = building.takesFullCell;
                    c.Walkable = building.walkable;
                }
            }
            
        }
    }
}
