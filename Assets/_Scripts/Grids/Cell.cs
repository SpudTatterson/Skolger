using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public string id;
    public int x, y; // index
    public GridManager grid; // the grid the cell belongs to
    public Vector3 position;
    public Cell(int x, int y, Vector3 position, GridManager grid)
    {
        this.x = x;
        this.y = y;
        this.position = position;
        this.grid = grid;
        this.id = x + "/" + y + " " + "Cell";
    }

    public override string ToString()
    {
        return id;
    }
}
