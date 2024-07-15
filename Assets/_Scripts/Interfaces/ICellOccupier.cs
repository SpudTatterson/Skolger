using UnityEngine;

public interface ICellOccupier 
{
    Cell cornerCell { get; }
    public void GetOccupiedCells();
    public void OnOccupy(); 
    public void OnRelease();
}