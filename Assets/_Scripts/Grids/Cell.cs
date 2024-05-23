using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public string id;
    public int x, y; // index
    public GridObject grid; // the grid the cell belongs to
    public Vector3 position; // position in the world
    public bool Walkable = true; // for Path Finding 
    public bool inUse = false; // does this cell have a building, tree, item etc on it
    public Cell(int x, int y, Vector3 position, GridObject grid)
    {
        this.x = x;
        this.y = y;
        this.position = position;
        this.grid = grid;
        this.id = x + "/" + y + " " + "Cell";
    }
    public bool IsFreeForBuilding()
    {
        return !inUse;
    }
    public static bool AreCellsFree(List<Cell> cells)
    {
        foreach (Cell cell in cells)
        {
            bool isFree = cell.IsFreeForBuilding();
            if (!isFree) return false;
        }
        return true;
    }
    public override string ToString()
    {
        return id;
    }
}
