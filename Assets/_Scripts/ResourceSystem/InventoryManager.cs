using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<Stockpile> stockpiles = new List<Stockpile>();
    public SerializableDictionary<ItemType, SerializableDictionary<ItemData, int>> totalItems = new SerializableDictionary<ItemType, SerializableDictionary<ItemData, int>>();
    public event Action<ItemData, int> OnInventoryUpdated;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            Debug.Log("More than one InventoryManager exists.");
        }
    }

    // Method to get stockpile with empty space
    public Stockpile GetStockpileWithEmptySpace(out Cell cell)
    {
        foreach (Stockpile stockpile in stockpiles)
        {
            if (stockpile.GetEmptyCell(out cell))
                return stockpile;
        }
        cell = null;
        return null;
    }

    Stockpile GetStockpileWithItem(ItemData itemData, int amount)
    {
        foreach (Stockpile stockpile in stockpiles)
        {
            if (stockpile.HasItem(itemData, amount))
                return stockpile;
        }
        Debug.Log("Didn't find item in any of the stockpiles.");
        return null;
    }

    public bool TryFindFoodInStockpiles(out EdibleData edibleData, out Stockpile stockpile, out Cell itemPosition)
    {
        edibleData = null;
        stockpile = null;
        itemPosition = null;

        if (!totalItems.ContainsKey(ItemType.Edible) || totalItems[ItemType.Edible] == null)
            return false;
        else
        {
            foreach (KeyValuePair<ItemData, int> foodItem in totalItems[ItemType.Edible])
            {
                if (foodItem.Value > 0)
                {
                    edibleData = (EdibleData)foodItem.Key;
                    stockpile = GetStockpileWithItem(edibleData, foodItem.Value);
                    itemPosition = stockpile.GetItemCell(edibleData, foodItem.Value);
                    return true;
                }
            }
            return false;
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        if (totalItems.TryGetValue(item.itemType, out SerializableDictionary<ItemData, int> itemDict))
        {
            if (itemDict.ContainsKey(item))
                itemDict[item] += amount;
            else
                itemDict.Add(item, amount);
        }
        else
        {
            var newItemDict = new SerializableDictionary<ItemData, int> { { item, amount } };
            totalItems.Add(item.itemType, newItemDict);
        }
        OnInventoryUpdated?.Invoke(item, amount);
    }

    public bool HasItem(ItemCost itemCost)
    {
        return totalItems.TryGetValue(itemCost.item.itemType, out SerializableDictionary<ItemData, int> itemDict) &&
               itemDict.TryGetValue(itemCost.item, out int currentAmount) &&
               currentAmount >= itemCost.cost;
    }

    public bool HasItems(List<ItemCost> itemCosts)
    {
        foreach (ItemCost cost in itemCosts)
        {
            if (!HasItem(cost))
            {
                Debug.Log("Missing: " + cost.ToString());
                return false;
            }
        }
        return true;
    }

    public InventoryItem TakeItem(ItemCost itemCost, Stockpile stockpile = null)
    {
        if (stockpile == null)
            stockpile = GetStockpileWithItem(itemCost.item, itemCost.cost);
        if (stockpile == null)
        {
            Debug.LogWarning("Item not found in stockpiles.");
            return null;
        }
        return stockpile.TakeItem(itemCost.item, itemCost.cost);
    }

    // Method to get item location
    public Cell GetItemLocation(ItemData itemData, int cost, out Stockpile stockpile)
    {
        stockpile = GetStockpileWithItem(itemData, cost);
        if (stockpile == null)
        {
            Debug.LogWarning("Item not found in stockpiles.");
            return null;
        }
        return stockpile.GetItemCell(itemData, cost);
    }

    public void RemoveAmountOfItem(ItemData item, int amount)
    {
        if (totalItems.TryGetValue(item.itemType, out SerializableDictionary<ItemData, int> itemDict) &&
            itemDict.TryGetValue(item, out int currentAmount))
        {
            itemDict[item] = Mathf.Max(0, currentAmount - amount);
        }
        OnInventoryUpdated?.Invoke(item, -amount);
    }

    public int CheckAmount(ItemData item)
    {
        if (totalItems.TryGetValue(item.itemType, out SerializableDictionary<ItemData, int> itemDict) &&
            itemDict.TryGetValue(item, out int currentAmount))
        {
            return currentAmount;
        }
        return 0;
    }

    [ContextMenu("CheckInv")]
    void CheckInv()
    {
        foreach (var itemType in totalItems)
        {
            foreach (var item in itemType.Value)
            {
                Debug.Log(item.Key + ": " + item.Value);
            }
        }
    }
}
