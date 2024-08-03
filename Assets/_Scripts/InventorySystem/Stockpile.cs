using System;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : MonoBehaviour, ISelectable, ICellOccupier
{
    int sizeX;
    int sizeY;
    [SerializeField] SerializableDictionary<Cell, ItemObject> cells = new SerializableDictionary<Cell, ItemObject>();
    [SerializeField] SerializableDictionary<Cell, GameObject> visualItems = new SerializableDictionary<Cell, GameObject>();
    [SerializeField] SerializableDictionary<ItemData, int> totalItems = new SerializableDictionary<ItemData, int>();
    List<Cell> emptyCells = new List<Cell>();
    List<Cell> occupiedCells = new List<Cell>();
    public Cell cornerCell { get; private set; }
    GameObject visual;
    Outline outline;


    public void Initialize(int sizeX, int sizeY, Cell cornerCell)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        cells.Clear();
        emptyCells.Clear();
        visualItems.Clear();

        this.cornerCell = cornerCell;
        cornerCell.grid.TryGetCells((Vector2Int)cornerCell, sizeX, sizeY, out List<Cell> occupiedCells);

        foreach (Cell cell in occupiedCells)
        {
            if (cell.IsFreeAndExists())
                AddCell(cell);

        }

        float cellSize = GridManager.instance.worldSettings.cellSize;
        // Calculate the position of the bottom-left corner of the cell
        Vector3 cornerPosition = cornerCell.position - new Vector3(cellSize / 2, 0, cellSize / 2);

        // Adjust position slightly below the ground for better visual separation
        transform.position = cornerPosition + new Vector3(0, 0.01f, 0);

        InventoryManager.instance.stockpiles.Add(this);

        // Create the grid mesh
        visual = MeshUtility.CreateGridMesh(this.occupiedCells, transform.position, "Stockpile", MaterialManager.instance.stockpileMaterial, transform, 1);
        outline = visual.AddComponent<Outline>();
        outline?.Disable();
    }
    public bool GetEmptyCell(out Cell cell)
    {
        cell = null;
        if (emptyCells.Count > 0)
        {
            cell = emptyCells[0];
            return true;
        }
        return false;
    }

    public void AddItem(Cell cell, IItem item)
    {
        if (cells.ContainsKey(cell) && cells[cell] == null)
        {
            cells[cell] = ItemObject.MakeInstance(item.itemData, item.amount, cell.position, true, transform, true, this);
            visualItems[cell] = cells[cell].gameObject;
            emptyCells.Remove(cell);
            InventoryManager.instance.AddItem(item.itemData, item.amount);
            if (totalItems.ContainsKey(item.itemData))
                totalItems[item.itemData] += item.amount;
            else
                totalItems.Add(item.itemData, item.amount);
        }
    }
    public Cell GetItemCell(ItemData itemData, int cost)
    {
        ItemObject foundItems = FindAndMergeItem(new ItemCost(itemData, cost));

        return foundItems.cornerCell;
    }
    public bool AddItem(IItem item)
    {
        if (HasItem(item.itemData) && item.itemData.stackSize != 1)
        {
            List<ItemObject> itemsInStockpile = FindItems(item.itemData);
            int amountToDistribute = item.amount;
            foreach (ItemObject stockpileItem in itemsInStockpile)
            {
                if (stockpileItem.amount + amountToDistribute <= item.itemData.stackSize)
                {
                    stockpileItem.MergeItem(item);
                    InventoryManager.instance.AddItem(item.itemData, amountToDistribute);
                    totalItems[item.itemData] += amountToDistribute;
                    return true;
                }
                else if (stockpileItem.amount + amountToDistribute > item.itemData.stackSize)
                {
                    int amountTaken = stockpileItem.MergeItem(item);
                    amountToDistribute -= amountTaken;
                    InventoryManager.instance.AddItem(item.itemData, amountTaken);
                    totalItems[item.itemData] += amountTaken;
                }
            }

        }
        Cell cell;
        if (!GetEmptyCell(out cell)) return false;
        if (cells.ContainsKey(cell) && cells[cell] == null)
        {
            cells[cell] = ItemObject.MakeInstance(item.itemData, item.amount, cell.position, true, transform, true, this);
            visualItems[cell] = cells[cell].gameObject;
            emptyCells.Remove(cell);
            InventoryManager.instance.AddItem(item.itemData, item.amount);
            if (totalItems.ContainsKey(item.itemData))
                totalItems[item.itemData] += item.amount;
            else
                totalItems.Add(item.itemData, item.amount);
        }
        return true;
    }


    public InventoryItem TakeItem(ItemData itemData, int amount, Vector3 position = default, Transform parent = null)
    {
        List<ItemObject> matchingItems = FindItems(itemData);
        int requiredAmount = amount;

        List<ItemObject> itemsToReturn = new List<ItemObject>();
        foreach (ItemObject item in matchingItems)
        {
            if (item.amount == requiredAmount)
            {
                RemoveItem(item);
                return item.PickUp();
            }
            else if (item.amount > requiredAmount)
            {
                ItemObject newItem = item.SplitItem(requiredAmount, position, parent);
                InventoryManager.instance.RemoveAmountOfItem(newItem.itemData, newItem.amount);
                totalItems[newItem.itemData] -= amount;
                return newItem.PickUp();
            }
            else if (item.amount < requiredAmount)
            {
                requiredAmount -= item.amount;
                itemsToReturn.Add(item);
                InventoryManager.instance.RemoveAmountOfItem(item.itemData, item.amount);
                totalItems[item.itemData] -= item.amount;
            }
        }
        // Create a combined item from the items we are taking
        if (requiredAmount == 0)
        {
            foreach (ItemObject item in itemsToReturn)
            {
                RemoveItem(item);
            }
            return new InventoryItem(itemData, amount);
        }

        // If we couldn't fulfill the request, put the items back
        foreach (ItemObject item in itemsToReturn)
        {
            InventoryManager.instance.AddItem(item.itemData, item.amount);
            totalItems[item.itemData] += item.amount;
        }
        return null;
    }
    public bool HasItem(ItemData itemData, int amount)
    {
        if (totalItems[itemData] >= amount) return true;

        Debug.Log("Not enough of " + itemData.name + " was found in the stockpile or item not found.");
        return false;
    }
    bool HasItem(ItemData itemData)
    {
        return totalItems.ContainsKey(itemData);
    }
    List<ItemObject> FindItems(ItemData itemData)
    {
        List<ItemObject> foundItems = new List<ItemObject>();
        foreach (KeyValuePair<Cell, ItemObject> pair in cells)
        {
            if (pair.Value != null && pair.Value.itemData == itemData)
            {
                foundItems.Add(pair.Value);
            }
        }
        return foundItems;
    }
    ItemObject FindAndMergeItem(ItemCost itemCost)
    {
        List<ItemObject> foundItems = new List<ItemObject>();
        int costToFulfil = itemCost.cost;
        foreach (KeyValuePair<Cell, ItemObject> pair in cells)
        {
            if (pair.Value != null && pair.Value.itemData == itemCost.item && costToFulfil >= 0)
            {
                foundItems.Add(pair.Value);
                costToFulfil -= pair.Value.amount;
            }
        }
        if (foundItems.Count == 1) return foundItems[0];

        ItemObject fullItem = foundItems[0];
        foundItems.RemoveAt(0);
        foreach (ItemObject item in foundItems)
        {
            fullItem.MergeItem(item);
        }
        return fullItem;
    }
    public void RemoveItem(ItemObject item)
    {
        Cell cell = null;
        foreach (var pair in cells)
        {
            if (pair.Value == item)
            {
                cell = pair.Key;
                break;
            }
        }

        if (cell != null)
        {
            cells[cell] = null;
            Destroy(visualItems[cell]);
            visualItems[cell] = null;
            emptyCells.Add(cell);
            InventoryManager.instance.RemoveAmountOfItem(item.itemData, item.amount);
            totalItems[item.itemData] -= item.amount;
        }
    }
    [ContextMenu("DestroyStockpile")]
    public void DestroyStockpile()
    {
        foreach (KeyValuePair<Cell, ItemObject> pair in cells)
        {
            if (pair.Value != null)
            {
                pair.Value.RemoveFromStockpile();
            }
        }
        foreach (Cell cell in emptyCells)
        {
            cell.inUse = false;
        }
        InventoryManager.instance.stockpiles.Remove(this);
        OnRelease();
        Destroy(this.gameObject);
    }

    public void ShrinkStockpile(List<Cell> cellsToRemove)
    {
        foreach (Cell cell in cellsToRemove)
        {
            if (emptyCells.Contains(cell))
            {
                RemoveCell(cell);
            }
        }
        if (occupiedCells.Count == 0)
            DestroyStockpile();

        MeshUtility.UpdateGridMesh(occupiedCells, visual.GetComponent<MeshFilter>());
    }
    public void GrowStockpile(List<Cell> cellsToAdd)
    {
        foreach (Cell cell in cellsToAdd)
        {
            if (!occupiedCells.Contains(cell) && cell.IsFreeAndExists())
            {
                AddCell(cell);
            }
        }

        MeshUtility.UpdateGridMesh(occupiedCells, visual.GetComponent<MeshFilter>());
    }

    void RemoveCell(Cell cell)
    {
        occupiedCells.Remove(cell);
        if (cells[cell] != null)
            cells[cell].RemoveFromStockpile();
        else
            cell.inUse = false;
        cells.Remove(cell);
        visualItems.Remove(cell);
        emptyCells.Remove(cell);

    }
    void AddCell(Cell cell)
    {
        occupiedCells.Add(cell);
        cells.Add(cell, null);
        visualItems.Add(cell, null);
        emptyCells.Add(cell);
        cell.inUse = true;
    }
    #region ISelectable

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.instance;
        manager.AddToCurrentSelected(this);

        outline?.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.instance;
        manager.RemoveFromCurrentSelected(this);
        manager.UpdateSelection();

        outline?.Disable();
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
        return SelectionType.Stockpile;
    }

    public ISelectionStrategy GetSelectionStrategy()
    {
        return new StockpileSelectionStrategy();
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return $"Stockpile {sizeX} x {sizeY}";
    }

    public bool HasActiveCancelableAction()
    {
        return false;
    }

    #endregion

    #region ICellOccupier

    public void GetOccupiedCells()
    {
        if (GridManager.instance == null)
            GridManager.InitializeSingleton();

        cornerCell = GridManager.instance.GetCellFromPosition(transform.position);
        cornerCell.grid.TryGetCells((Vector2Int)cornerCell, sizeX, sizeY, out List<Cell> occupiedCells);
        this.occupiedCells = occupiedCells;
    }
    public void OnOccupy()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = true;
        }
    }

    public void OnRelease()
    {
        foreach (Cell cell in occupiedCells)
        {
            cell.inUse = false;
        }
    }

    #endregion

    void OnDisable()
    {
        OnDeselect();
    }
}
