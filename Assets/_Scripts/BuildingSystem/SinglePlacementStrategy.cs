using System.Collections.Generic;
using UnityEngine;

public class SinglePlacementStrategy : IPlacementStrategy
{
    public List<Cell> GetCells(Cell firstCell, Cell lastCell)
    {
        List<Cell> cells = new List<Cell>
        {
            firstCell
        };
        return cells;
    }
}