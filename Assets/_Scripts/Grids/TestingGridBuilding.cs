using System.Collections.Generic;
using UnityEngine;

public class TestingGridBuilding : MonoBehaviour
{
    public GameObject testGO;
    public List<Cell> cells;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {

            Debug.Log(hit.transform.name);
            GridManager grid = hit.transform.GetComponentInParent<GridManager>();
            if(grid == null) Debug.Log("No grid found");
            Cell cell = grid.GetCellFromPosition(hit.point);
            if(cell == null) Debug.Log("No cell found");
            Debug.Log(cell.position);
            if(grid.TryGetCells(new Vector2Int(cell.x, cell.y), 2,2, out cells))
            {
                foreach (Cell c in cells)
                {
                    Instantiate(testGO, c.position, Quaternion.identity);
                }
            }
            
        }
    }
}
