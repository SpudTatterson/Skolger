using System.Collections.Generic;
using UnityEngine;

public interface IPlacementStrategy
{
    List<Cell> GetCells(Cell firstCell, Cell lastCell);
}