using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class BuildingObject : MonoBehaviour, ISelectable, ICellOccupier
{
    [SerializeField, LabelText("Building Data"), InlineEditor] BuildingData data;
    [field: SerializeField, ReadOnly, InlineEditor] public BuildingData buildingData { get; private set; }
    [SerializeField, ReadOnly] Direction placementDirection = Direction.TopLeft;
    List<Cell> occupiedCells;
    [field: SerializeField, ReadOnly] public Cell cornerCell { get; private set; }

    public bool IsSelected { get; private set; }

    Outline outline;


    void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();
    }

    public void Initialize(BuildingData buildingData, Direction placementDirection)
    {
        this.buildingData = buildingData;
        this.placementDirection = placementDirection;
        transform.rotation = Quaternion.Euler(0, (int)placementDirection, 0);
        GetOccupiedCells();

        OnOccupy();

        if (!TryGetComponent(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline?.Disable();
    }

    public static BuildingObject MakeInstance(BuildingData buildingData, Vector3 position, Direction placementDirection, Transform parent = null)
    {
        GameObject buildingVisual;
#if UNITY_EDITOR
        buildingVisual = PrefabUtility.InstantiatePrefab(buildingData.buildingPrefab) as GameObject;
#else
        buildingVisual = GameObject.Instantiate(buildingData.buildingPrefab);
#endif
        buildingVisual.transform.position = position;
        buildingVisual.transform.parent = parent;
        if (!buildingVisual.TryGetComponent(out BuildingObject building))
            building = buildingVisual.AddComponent<BuildingObject>();
#if UNITY_EDITOR
        EditorUtility.SetDirty(building);
#endif
        building.Initialize(buildingData, placementDirection);

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

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.AddToCurrentSelected(this);
        IsSelected = true;

        outline?.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.RemoveFromCurrentSelected(this);
        if (IsSelected)
            manager.UpdateSelection();

        outline?.Disable();
        IsSelected = false;
    }

    public void OnHover()
    {
        outline?.Enable();
    }

    public void OnHoverEnd()
    {
        outline?.Disable();
    }

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
        if (buildingData == null)
            buildingData = data;

        cornerCell = GridManager.Instance.GetCellFromPosition(transform.position);
        cornerCell.grid.TryGetCells((Vector2Int)cornerCell, buildingData.xSize, buildingData.ySize, out List<Cell> occupiedCells, placementDirection);
        this.occupiedCells = occupiedCells;
    }
    public void OnOccupy()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = buildingData.usesCell;
            cell.walkable = buildingData.walkable;
            if (buildingData is FloorTile) cell.hasFloor = true;
        }
    }

    public void OnRelease()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = false;
            cell.walkable = true;
            if (buildingData is FloorTile) cell.hasFloor = false;
        }
    }

    #endregion

    void OnDisable()
    {
        OnDeselect();
    }
}
