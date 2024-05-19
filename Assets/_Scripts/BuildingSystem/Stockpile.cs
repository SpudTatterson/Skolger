using System.Collections.Generic;
using UnityEngine;

public class Stockpile : MonoBehaviour
{
    int sizeX;
    int sizeY;
    [SerializeField] SerializableDictionary<Cell, ItemObject> cells = new SerializableDictionary<Cell, ItemObject>();
    [SerializeField] SerializableDictionary<Cell, GameObject> visualItems = new SerializableDictionary<Cell, GameObject>();
    [SerializeField] SerializableDictionary<ItemData, int> totalItems = new SerializableDictionary<ItemData, int>();
    List<Cell> emptyCells = new List<Cell>();


    public void Initialize(int sizeX, int sizeY, List<Cell> occupiedCells, Vector3 cellPosition, float cellSize = 1)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        cells.Clear();
        emptyCells.Clear();
        visualItems.Clear();

        foreach (Cell cell in occupiedCells)
        {
            cells.Add(cell, null);
            visualItems.Add(cell, null);
            emptyCells.Add(cell);
        }

        // Calculate the position of the bottom-left corner of the cell
        Vector3 cornerPosition = cellPosition - new Vector3(cellSize / 2, 0, cellSize / 2);

        // Adjust position slightly below the ground for better visual separation
        transform.position = cornerPosition + new Vector3(0, 0.01f, 0);

        InventoryManager.instance.stockpiles.Add(this);

        // Create the grid mesh
        MeshUtility.CreateGridMesh(sizeX, sizeY, transform.position, "Stockpile", MaterialManager.instance.stockpileMaterial, transform);
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

    // add method to stack similar items in to 1 stack

    public void AddItem(Cell cell, ItemObject item)
    {
        if (cells.ContainsKey(cell) && cells[cell] == null)
        {
            cells[cell] = ItemObject.MakeInstance(item.itemData, item.amount, cell.position, transform, true, this);
            visualItems[cell] = cells[cell].gameObject;
            emptyCells.Remove(cell);
            InventoryManager.instance.AddItem(item.itemData, item.amount);
            if (totalItems.ContainsKey(item.itemData))
                totalItems[item.itemData] += item.amount;
            else
                totalItems.Add(item.itemData, item.amount);
        }
    }
    public bool AddItem(ItemObject item)
    {
        if (HasItem(item.itemData) && item.itemData.stackSize != 1)
        {
            Debug.Log("has item in inv");
            List<ItemObject> itemsInStockpile = FindItems(item.itemData);
            int amountToDistribute = item.amount;
            foreach (ItemObject stockpileItem in itemsInStockpile)
            {

                if (stockpileItem.amount + amountToDistribute <= item.itemData.stackSize)
                {
                    stockpileItem.MergeItem(item, Vector3.zero);
                    InventoryManager.instance.AddItem(item.itemData, amountToDistribute);
                    totalItems[item.itemData] += amountToDistribute;
                    return true;
                }
                else if (stockpileItem.amount + amountToDistribute > item.itemData.stackSize)
                {
                    int amountTaken = stockpileItem.MergeItem(item, Vector3.zero);
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
            cells[cell] = ItemObject.MakeInstance(item.itemData, item.amount, cell.position, transform, true, this);
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

    // void MergeItems(ItemObject itemInStockpile, ItemObject item)
    // {
    //     int amountToMove = Mathf.Min(item.itemData.stackSize - itemInStockpile.amount, item.amount);
    //     item.UpdateAmount(-amountToMove);
    //     itemInStockpile.UpdateAmount(amountToMove);
    //     totalItems[item.itemData] += amountToMove;
    //     InventoryManager.instance.AddItem(item.itemData, amountToMove);
    //     Debug.Log($"Merged {amountToMove} items. Stockpile now has {itemInStockpile.amount}, item has {item.amount}");
    // }

    public List<ItemObject> TakeItem(ItemData itemData, int amount, Vector3 position = default, Transform parent = null)
    {
        List<ItemObject> matchingItems = FindItems(itemData);
        int requiredAmount = amount;

        List<ItemObject> itemsToTake = new List<ItemObject>();

        foreach (ItemObject item in matchingItems)
        {
            if (item.amount == requiredAmount)
            {
                itemsToTake.Add(item);
                RemoveItem(item);
                return itemsToTake;
            }
            else if (item.amount > requiredAmount)
            {
                ItemObject newItem = item.SplitItem(requiredAmount, position, parent);
                itemsToTake.Add(newItem);
                return itemsToTake;
            }
            else if (item.amount < requiredAmount)
            {
                itemsToTake.Add(item);
                requiredAmount -= item.amount;
                RemoveItem(item);
            }
        }
        return itemsToTake;
    }
    public bool HasItem(ItemData itemData, int amount)
    {

        bool existInInv = InventoryManager.instance.HasItem(new ItemCost(itemData, amount));
        if (!existInInv) return false;
        existInInv = totalItems[itemData] != 0f;
        if (!existInInv) return false;
        foreach (KeyValuePair<Cell, ItemObject> pair in cells)
        {
            if (pair.Value != null && pair.Value.itemData == itemData)
            {
                if (pair.Value.amount <= amount)
                {
                    return true;
                }
            }
        }

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
}
