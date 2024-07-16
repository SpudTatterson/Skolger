using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class BuildingObject : MonoBehaviour, ISelectable, ICellOccupier
{
    [field: SerializeField, ReadOnly, Expandable] public BuildingData buildingData { get; private set; }
    List<Cell> occupiedCells;
    [field: SerializeField, ReadOnly] public Cell cornerCell { get; private set; }

    public void Initialize(BuildingData buildingData, List<Cell> occupiedCells)
    {
        this.buildingData = buildingData;
        this.occupiedCells = occupiedCells;

        OnOccupy();
    }
    // add method for deconstructing
    public static BuildingObject MakeInstance(BuildingData buildingData, Vector3 position, List<Cell> occupiedCells, Transform parent = null)
    {
        GameObject buildingVisual = PrefabUtility.InstantiatePrefab(buildingData.buildingPrefab) as GameObject;
        buildingVisual.transform.position = position;
        buildingVisual.transform.parent = parent;

        BuildingObject building = buildingVisual.AddComponent<BuildingObject>();
        EditorUtility.SetDirty(building);
        building.Initialize(buildingData, occupiedCells);

        return building;
    }
    public void Deconstruct()
    {
        foreach (ItemCost cost in buildingData.costs)
        {
            int stackSize = cost.item.stackSize;
            Cell cell;
            if (cost.cost > stackSize)
            {
                int costToDisperse = cost.cost;
                while (costToDisperse > stackSize)
                {
                    cell = occupiedCells[0].GetClosestEmptyCell();

                    ItemObject.MakeInstance(cost.item, stackSize, cell.position);
                    costToDisperse -= stackSize;
                }
                cell = occupiedCells[0].GetClosestEmptyCell();
                ItemObject.MakeInstance(cost.item, costToDisperse, cell.position);
            }
            else
            {
                cell = occupiedCells[0].GetClosestEmptyCell();
                ItemObject.MakeInstance(cost.item, cost.cost, cell.position);
            }
        }
        OnRelease();
        Destroy(gameObject);
    }

    #region ISelectable

    public SelectionType GetSelectionType()
    {
        return SelectionType.Building;
    }
    public ISelectionStrategy GetSelectionStrategy()
    {
        return new BuildingSelectionStrategy();
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return buildingData.placeableName;
    }

    public bool HasActiveCancelableAction()
    {
        return false;
    }

    #endregion

    #region ICellOccupier

    public void GetOccupiedCells()
    {
        cornerCell = FindObjectOfType<GridManager>().GetCellFromPosition(transform.position);
        cornerCell.grid.TryGetCells((Vector2Int)cornerCell, buildingData.xSize, buildingData.ySize, out List<Cell> occupiedCells);
        this.occupiedCells = occupiedCells;
    }
    public void OnOccupy()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = buildingData.takesFullCell;
            cell.walkable = buildingData.walkable;
        }
    }

    public void OnRelease()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = false;
            cell.walkable = true;
        }
    }

    #endregion

}
