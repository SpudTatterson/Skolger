using System.Collections.Generic;
using UnityEngine;

public class SquarePlacementStrategy : IPlacementStrategy
{
    public List<Cell> GetCells(Cell firstCell, Cell lastCell)
    {
        (Vector2Int cellAmount, Cell cornerCell) = GridObject.GetGridBoxFrom2Cells(firstCell, lastCell);

        cornerCell.grid.TryGetCells((Vector2Int)cornerCell, cellAmount.x, cellAmount.y, out List<Cell> cells);

        return cells;
    }
}