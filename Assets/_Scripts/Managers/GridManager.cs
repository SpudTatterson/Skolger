using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    public List<GridObject> grids;
    
    [RequiredIn(PrefabKind.InstanceInScene, ErrorMessage = "Please attach the grids empty parent to generate the world map"),]
    public GameObject GridsParent;

    [InlineEditor] public WorldSettings worldSettings;

    // get these values from a scriptable object 
    public NavMeshSurface navMeshSurface { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GetGridsIfMissing();
        RecalculateCellUsage();
    }
    [ContextMenu("test")]
    void test()
    {
        Debug.Log(GridsParent == null);
        Debug.Log(Instance.GridsParent == null);
        GetGridsIfMissing();
    }
    public GridObject GetGridFromPosition(Vector3 position)
    {
        GetGridsIfMissing();
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

    void GetGridsIfMissing()
    {
        if (GridsParent == null) GridsParent = FindObjectOfType<NavMeshSurface>().gameObject;
        if (grids == null || grids.Count == 0)
            grids = GridsParent.GetComponentsInChildren<GridObject>().ToList();
    }

    public Cell GetCellFromPosition(Vector3 position)
    {
        if (GetGridFromPosition(position) == null) return null;
        return GetGridFromPosition(position).GetCellFromPosition(position);

    }
    [ContextMenu("GenerateWorld"), Button, GUIColor("Red")]
    public void GenerateWorld()
    {
        GridsParent.transform.position = Vector3.zero;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        DestroyOldWorld();
        grids.Add(GridObject.MakeInstance(worldSettings, false, GridsParent.transform.position, GridsParent.transform, "FloorGrid"));

        for (int i = 0; i < worldSettings.belowGroundLayers; i++)
        {
            Vector3 position = GridsParent.transform.position - Vector3.up * worldSettings.cellHeight * (i + 1);
            grids.Add(GridObject.MakeInstance(worldSettings, false, position, GridsParent.transform, $"Underground Grid Layer #{i}"));
        }
        for (int i = 0; i < worldSettings.aboveGroundLayers; i++)
        {
            Vector3 position = GridsParent.transform.position + Vector3.up * worldSettings.cellHeight * (i + 1);
            grids.Add(GridObject.MakeInstance(worldSettings, true, position, GridsParent.transform, $"Aboveground Grid Layer #{i}"));
        }

        RebuildNavmesh();
    }

    [Button, GUIColor("Green")]
    public void UpdateAllGrids()
    {
        GetGridsIfMissing();
        foreach (GridObject grid in grids)
        {
            grid.UpdateVisualGrid();
        }
    }

    [Button, GUIColor("Green")]
    public void RebuildNavmesh()
    {
        if (navMeshSurface == null)
        {
            if (!GridsParent.TryGetComponent(out NavMeshSurface navMeshSurface))
            {
                navMeshSurface = GridsParent.AddComponent<NavMeshSurface>();
            }
            this.navMeshSurface = navMeshSurface;
        }
        navMeshSurface.collectObjects = CollectObjects.Children;
        navMeshSurface.BuildNavMesh();
    }

    [Button, GUIColor("Green")]
    public void RecalculateCellUsage()
    {
        GetGridsIfMissing();
        foreach (GridObject grid in grids)
        {
            grid.ResetCellUse();
        }
        List<ICellOccupier> cellOccupiers = FindObjectsOfType<MonoBehaviour>().OfType<ICellOccupier>().ToList();
        foreach (ICellOccupier occupier in cellOccupiers)
        {
            occupier.GetOccupiedCells();
            occupier.OnOccupy();
        }
    }

    [Button, GUIColor("Yellow")]
    public void SaveAllGridMeshesToFile()
    {
        GetGridsIfMissing();
        foreach (GridObject grid in grids)
        {
            grid.SaveAllChunkMeshesToFile();
        }
    }
    [Button, GUIColor("Yellow")]
    public void LoadAllGridMeshesFromFile()
    {
        GetGridsIfMissing();
        foreach (GridObject grid in grids)
        {
            grid.LoadChunksFromFile();
        }
    }
    private void DestroyOldWorld()
    {
        grids = GridsParent.GetComponentsInChildren<GridObject>().ToList();

        foreach (var grid in grids)
        {
            DestroyImmediate(grid.gameObject);
        }
        grids.Clear();
    }

    // make method to find all ICellOccupiers and make them find the cells they sit on and update the cells as needed
}
