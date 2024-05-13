using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Cell : MonoBehaviour
{
    public Vector3 center { get; private set; }
    public bool Placed = false;


    [Header("References")]

    MeshRenderer mr;
    Bounds bounds;
    public int x { get; private set; } // both x and y vars are for storing the index of the cell in the grid
    public int y { get; private set; }
    public GridManager grid { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        mr = GetComponentInChildren<MeshRenderer>();
        GetCenterPoint();
    }
    void Update()
    {
        if(Placed ) mr.material.color = Color.blue;
    }
    public void ReconnectWithGrid()
    {
        if (grid == null) grid = GetComponentInParent<GridManager>();
        if (grid.cells == null) grid.InitializeCells();

        grid.cells[x, y] = this;
    }
    void GetCenterPoint()
    {
        bounds = mr.bounds;
        bounds.extents = Vector3.Scale(bounds.extents, transform.localScale); //Apply the GameObject's scale to the bounds extents
        center = bounds.center + new Vector3(0, bounds.extents.y, 0);
    }
    public void SetGridManager(GridManager grid)
    {
        this.grid = grid;
    }

    public void SetXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[InitializeOnLoad]
public class GridReloader
{
    static GridReloader()
    {
        EditorApplication.delayCall += ReconnectCells;
    }

    static void ReconnectCells()
    {
        foreach (var cell in Object.FindObjectsOfType<Cell>())
        {
            cell.ReconnectWithGrid();
        }
    }
}