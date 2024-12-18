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
    public bool walkable = true; // for Path Finding 
    public bool inUse = false; // does this cell have a building, tree, item etc on it
    public bool hasFloor = false;
    public bool isVisible = true;
    public bool inFullUse = false;
    public CellType cellType;

    public Cell(int x, int y, bool isVisible, Vector3 position, GridObject grid)
    {
        this.x = x;
        this.y = y;
        this.isVisible = isVisible;
        this.position = position;
        this.grid = grid;
        cellType = CellType.Grass;
        this.id = x + "/" + y + " " + "Cell";
    }
    public bool IsFreeAndExists()
    {
        if (IsFree() && (isVisible || inFullUse))
            return true;
        else
            return false;
    }

    public bool IsFree()
    {
        bool isCellAboveVisible = GetCellAbove().isVisible;
        if (!inUse && !isCellAboveVisible)
            return true;
        else
            return false;
    }
    public bool IsRoofed()
    {
        Cell cellAbove = GetCellAbove();

        if (cellAbove.isVisible || cellAbove.hasFloor)
            return true;
        else
            return false;
    }
    public void SetUseAndWalkable(bool inUse, bool walkable)
    {
        this.inUse = inUse;
        this.walkable = walkable;
    }
    public static bool AreCellsFree(List<Cell> cells)
    {
        foreach (Cell cell in cells)
        {
            bool isFree = cell.IsFreeAndExists();
            if (!isFree) return false;
        }
        return true;
    }
    public Cell GetClosestEmptyCell()
    {
        if (!this.isVisible)
            return GetCellBelow()?.GetClosestEmptyCell();

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

            if (currentCell != null && currentCell.IsFreeAndExists())
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

        Debug.Log("No empty cell found on grid \n trying to find empty cell on grid below");

        return GetCellBelow()?.GetClosestEmptyCell();
    }
    public Cell GetCellAbove()
    {
        return GridManager.Instance.GetCellFromPosition(position + new Vector3(0, GridManager.Instance.worldSettings.cellHeight, 0));
    }
    public Cell GetCellBelow()
    {
        return GridManager.Instance.GetCellFromPosition(position - new Vector3(0, GridManager.Instance.worldSettings.cellHeight, 0));
    }
    public override string ToString()
    {
        return $"{id}, {grid.gameObject.name} \n {cellType} \n in use {inUse} \n has floor {hasFloor} \n in full use {inFullUse} \n is visible {isVisible}";
    }
    public static explicit operator Vector2Int(Cell cell)
    {
        return new Vector2Int(cell.x, cell.y);
    }
}

public enum CellType
{
    Grass,
    Rock,
    Sand,
    Dirt
}