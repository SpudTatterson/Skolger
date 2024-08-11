using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ConstructionSiteObject : MonoBehaviour, IConstructable, ISelectable, IAllowable, ICellOccupier
{
    [SerializeField, Label("Building Data"), Expandable] BuildingData data;
    [field: SerializeField, ReadOnly, Expandable] public BuildingData buildingData { get; private set; }
    List<ItemCost> costs = new List<ItemCost>();
    SerializableDictionary<ItemData, int> fulfilledCosts = new SerializableDictionary<ItemData, int>();
    List<Cell> occupiedCells = new List<Cell>();
    public Cell cornerCell { get; private set; }
    bool allowed = true;
    [SerializeField, Tooltip("Should be set to true if manually placed in world")] bool manualInit = false;
    public bool SetForCancellation { get; private set; }
    public bool IsSelected { get; private set; }

    Outline outline;

    BillBoard forbiddenBillboard;
    // should probably hold ref to colonist that is supposed to build incase of canceling action + so that there wont be 2 colonists working on the same thing

    public void Initialize(BuildingData buildingData)
    {
        this.buildingData = buildingData;

        GetOccupiedCells();
        OnOccupy();

        costs.Clear();
        fulfilledCosts.Clear();
        foreach (ItemCost cost in this.buildingData.costs)
        {
            costs.Add(cost);
            if (fulfilledCosts.ContainsKey(cost.item))
                continue;
            else
                fulfilledCosts.Add(cost.item, 0);
        }

        // get necessary components
        forbiddenBillboard = GetComponentInChildren<BillBoard>(true);
        if (!TryGetComponent(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline?.Disable();

    }

    public static ConstructionSiteObject MakeInstance(BuildingData buildingData, Cell cell, Transform parent = null, bool temp = false)
    {
        GameObject buildingGO = PoolManager.Instance.GetObject(buildingData.unplacedVisual, cell.position, parent);

        if (!buildingGO.TryGetComponent(out ConstructionSiteObject building))
        {
            building = buildingGO.AddComponent<ConstructionSiteObject>();
        }

        if (!temp)
            building.Initialize(buildingData);


        return building;
    }

    void Start()
    {

        if (manualInit)
            Initialize(data);
    }

    #region Construction

    public void AddItem(InventoryItem item)
    {
        fulfilledCosts[item.itemData] += costs[0].cost;

        item.UpdateAmount(-costs[0].cost);

        if (item != null && item.amount > 0)
        {
            item.DropItem(cornerCell.GetClosestEmptyCell().position);
        }

        costs.RemoveAt(0);
    }
    public ItemCost GetNextCost()
    {
        if (costs.Count != 0)
            return costs[0];
        return null;
    }
    public List<ItemCost> GetAllCosts()
    {
        return costs;
    }
    public Cell GetPosition()
    {
        return cornerCell;
    }

    public bool CheckIfCostsFulfilled()
    {
        if (costs.Count == 0)
        {
            return true;
        }
        return false;
    }

    public void ConstructBuilding()
    {
        PoolManager.Instance.ReturnObject(buildingData.unplacedVisual, gameObject);
        BuildingObject.MakeInstance(buildingData, this.transform.position);
    }
    [ContextMenu("CancelConstruction")]
    public void CancelConstruction()
    {
        TaskManager.Instance.RemoveFromConstructionQueue(this);
        SetForCancellation = true;
        foreach (KeyValuePair<ItemData, int> cost in fulfilledCosts)
        {
            int stackSize = cost.Key.stackSize;
            Cell cell;
            if (cost.Value == 0) continue;

            if (cost.Value > stackSize)
            {
                int costToDisperse = cost.Value;
                while (costToDisperse > stackSize)
                {
                    cell = occupiedCells[0].GetClosestEmptyCell();

                    ItemObject.MakeInstance(cost.Key, stackSize, cell.position);
                    costToDisperse -= stackSize;
                }
                cell = occupiedCells[0].GetClosestEmptyCell();
                ItemObject.MakeInstance(cost.Key, costToDisperse, cell.position);
            }
            else
            {
                cell = occupiedCells[0].GetClosestEmptyCell();
                ItemObject.MakeInstance(cost.Key, cost.Value, cell.position);
            }
        }
        PoolManager.Instance.ReturnObject(buildingData.unplacedVisual, gameObject);
    }

    #endregion

    #region Selection

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.instance;
        manager.AddToCurrentSelected(this);
        IsSelected = true;

        outline?.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.instance;
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
        return SelectionType.Constructable;
    }

    public ISelectionStrategy GetSelectionStrategy()
    {
        return new ConstructableSelectionStrategy();
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return buildingData.placeableName + "(Construction Site)";
    }

    public bool HasActiveCancelableAction()
    {
        return true;
    }

    #endregion

    #region IAllowable

    public void OnAllow()
    {
        allowed = true;
        // add to construction queue
        TaskManager.Instance.AddToConstructionQueue(this);
        DisableBillboard();
    }

    public void OnDisallow()
    {
        allowed = false;
        // remove from construction queue
        TaskManager.Instance.RemoveFromConstructionQueue(this);
        // visually show that this is disallowed 
        EnableBillboard();
    }

    void EnableBillboard()
    {
        forbiddenBillboard.gameObject.SetActive(true);
    }
    void DisableBillboard()
    {
        forbiddenBillboard.gameObject.SetActive(false);
    }
    public bool IsAllowed()
    {
        return allowed;
    }

    #endregion

    #region ICellOccupier

    public void GetOccupiedCells()
    {
        if (GridManager.instance == null)
            GridManager.InitializeSingleton();

        cornerCell = GridManager.instance.GetCellFromPosition(transform.position);
        cornerCell.grid.TryGetCells((Vector2Int)cornerCell, buildingData.xSize, buildingData.ySize, out List<Cell> occupiedCells);
        this.occupiedCells = occupiedCells;
    }

    public void OnOccupy()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = buildingData.usesCell;
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

    void OnEnable()
    {
        OnOccupy();
    }
    void OnDisable()
    {
        OnRelease();

        OnDeselect();
    }
}
