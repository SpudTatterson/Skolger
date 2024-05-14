using System.Collections.Generic;
using UnityEngine;

public class TestingGridBuilding : MonoBehaviour
{
    public List<Cell> test;


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cell cell = hit.transform.GetComponentInParent<Cell>();
            if(cell == null) Debug.Log("Cell null");
            if(cell.grid == null) cell.ReconnectWithGrid();
            if(cell.grid.GetGrids(new Vector2Int(cell.x, cell.y), 2, 2, out test) == false)  Debug.Log("NO space");
            foreach (Cell c in test)
            {
                c.Placed = true;
                c.UpdateColor(Color.black);
            }
        }
    }
}