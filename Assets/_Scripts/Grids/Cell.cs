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
    public Cell GetClosestEmptyCell()
    {
        Queue<Vector2Int> toExplore = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        Vector2Int start = new Vector2Int(x, y);
        toExplore.Enqueue(start);
        visited.Add(start);

        while (toExplore.Count > 0)
        {
            Vector2Int current = toExplore.Dequeue();
            Cell currentCell = grid.GetCellFromIndex(current.x, current.y);

            if (currentCell != null && currentCell.IsFreeForBuilding())
            {
                return currentCell;
            }

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;

                if (!visited.Contains(neighbor))
                {
                    Cell neighborCell = grid.GetCellFromIndex(neighbor.x, neighbor.y);
                    if (neighborCell != null)
                    {
                        toExplore.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        Debug.Log("No empty cell found");
        return null; // No free cell found
    }
    public override string ToString()
    {
        return id;
    }
}
