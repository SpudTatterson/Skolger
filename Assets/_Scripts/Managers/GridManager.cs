using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
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
            // Calculate the Y range for the grid
            float gridMinY = grid.transform.position.y - worldSettings.cellHeight / 2;
            float gridMaxY = gridMinY + worldSettings.cellHeight;

            // Check if the position is within the grid's Y range
            if (position.y >= gridMinY && position.y < gridMaxY)
                return grid;
        }
        return null;
    }
    public Cell GetCellFromPosition(Vector3 position)
    {
        if (GetGridFromPosition(position) == null) return null;
        return GetGridFromPosition(position).GetCellFromPosition(position);

    }
    [ContextMenu("GenerateWorld"), Button]
    public void GenerateWorld()
    {
        DestroyOldWorld();
        grids.Add(GridObject.MakeInstance(worldSettings, false, gridsParent.transform.position, gridsParent.transform, "FloorGrid"));

        for (int i = 0; i < worldSettings.belowGroundLayers; i++)
        {
            Vector3 position = gridsParent.transform.position - Vector3.up * worldSettings.cellHeight * (i + 1);
            grids.Add(GridObject.MakeInstance(worldSettings, false, position, gridsParent.transform, $"Underground Grid Layer #{i}"));
        }
        for (int i = 0; i < worldSettings.aboveGroundLayers; i++)
        {
            Vector3 position = gridsParent.transform.position + Vector3.up * worldSettings.cellHeight * (i + 1);
            grids.Add(GridObject.MakeInstance(worldSettings, true, position, gridsParent.transform, $"Aboveground Grid Layer #{i}"));
        }
        EditorUtility.SetDirty(this);
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

    [Button]
    public void SaveAllGridMeshesToFile()
    {
        foreach (GridObject grid in grids)
        {
            grid.SaveAllChunkMeshesToFile();
        }
    }
    [Button]
    public void LoadAllGridMeshesFromFile()
    {
        foreach(GridObject grid in grids)
        {
            grid.LoadChunksFromFile();
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
