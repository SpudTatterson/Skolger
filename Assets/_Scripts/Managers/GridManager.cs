using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }
    public List<GridObject> grids { get; private set; }
    [SerializeField] GameObject gridsParent;

    [Expandable] public WorldSettings worldSettings;

    // get these values from a scriptable object 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.Log("More then one grid manager");
            Destroy(this);
        }

        grids = gridsParent.GetComponentsInChildren<GridObject>().ToList();

    }
    public GridObject GetGridFromPosition(Vector3 position)
    {
        if (grids == null || grids.Count == 0)
            grids = FindObjectsOfType<GridObject>().ToList();

        foreach (GridObject grid in grids)
        {
            if (Mathf.Round(grid.transform.position.y) == Mathf.Round(position.y))
                return grid;
        }
        return null;
    }
    public Cell GetCellFromPosition(Vector3 position)
    {
        if (GetGridFromPosition(position) == null) return null;
        return GetGridFromPosition(position).GetCellFromPosition(position);

    }
    [Button]
    public void RecalculateCellUsage()
    {
        foreach (GridObject grid in grids)
        {
            grid.ResetCellUse();
        }
        List<ICellOccupier> cellOccupiers = FindObjectsOfType<MonoBehaviour>(true).OfType<ICellOccupier>().ToList();
        foreach (ICellOccupier occupier in cellOccupiers)
        {
            occupier.GetOccupiedCells();
            occupier.OnOccupy();
        }
    }
    [ContextMenu("GenerateWorld"), Button]
    public void GenerateWorld()
    {
        DestroyOldWorld();
        GridObject.MakeInstance(worldSettings, false, gridsParent.transform.position, gridsParent.transform, "FloorGrid");

        for (int i = 0; i < worldSettings.belowGroundLayers; i++)
        {
            Vector3 position = gridsParent.transform.position - Vector3.up * worldSettings.cellHeight * (i + 1);
            GridObject.MakeInstance(worldSettings, false, position, gridsParent.transform, $"Underground Grid Layer #{i}");
        }
        for (int i = 0; i < worldSettings.aboveGroundLayers; i++)
        {
            Vector3 position = gridsParent.transform.position + Vector3.up * worldSettings.cellHeight * (i + 1);
            GridObject.MakeInstance(worldSettings, true, position, gridsParent.transform, $"Aboveground Grid Layer #{i}");
        }
    }

    private void DestroyOldWorld()
    {
        grids = gridsParent.GetComponentsInChildren<GridObject>().ToList();

        foreach (var grid in grids)
        {
            DestroyImmediate(grid.gameObject);
        }
        grids.Clear();
    }

    // make method to find all ICellOccupiers and make them find the cells they sit on and update the cells as needed
}
