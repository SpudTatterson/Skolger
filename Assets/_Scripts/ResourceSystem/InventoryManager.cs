using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<Stockpile> stockpiles = new List<Stockpile>();
    public SerializableDictionary<ItemData, int> totalItems = new SerializableDictionary<ItemData, int>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            Debug.Log("more the one inventory manager exists");
        }

    }

    // make method to get stockpile with empty space    
    Stockpile GetStockpileWithItem(ItemData itemData, int amount)
    {
        foreach (Stockpile stockpile in stockpiles)
        {
            bool existsInStock = stockpile.HasItem(itemData, amount);
            if(existsInStock) return stockpile;
        }
        Debug.Log("didn't find item in any of the stockpiles");
        return null;
    }
    public void AddItem(ItemData item, int amount)
    {
        if(totalItems.ContainsKey(item))
            totalItems[item] += amount;
        else
            totalItems.Add(item, amount);
    }

    public bool HasItem(ItemCost itemCost)
    {
        if (!totalItems.ContainsKey(itemCost.item)) return false;
        if (totalItems[itemCost.item] >= itemCost.cost) return true;
        else return false;
    }
    public bool HasItems(List<ItemCost> itemCosts)
    {
        foreach (ItemCost Cost in itemCosts)
        {
            bool has;
            has = HasItem(Cost);
            if (has == false)
            {
                Debug.Log("missing: " + Cost.ToString());
                return false;
            }
        }
        return true;
    }
    public List<ItemObject> TakeItem(ItemCost ItemCost)
    {
        Stockpile stockpile = GetStockpileWithItem(ItemCost.item, ItemCost.cost);
        List<ItemObject> itemToTake = stockpile.TakeItem(ItemCost.item, ItemCost.cost);
        return itemToTake;
       
    }
    public List<ItemObject> TakeItems(List<ItemCost> ItemCosts)
    {
        List<ItemObject> itemToTake = new List<ItemObject>();
        foreach (ItemCost cost in ItemCosts)
        {
            List<ItemObject> items = TakeItem(cost);
            foreach (ItemObject item in items)
            {
                itemToTake.Add(item);
            }
        }
        return itemToTake;
    }
    public List<Vector3> GetItemLocations(ItemData itemData, int cost)
    {
        List<Cell> cells = GetStockpileWithItem(itemData, cost).GetItemsCells(itemData, cost);
        List<Vector3> positions = new List<Vector3>();

        foreach (Cell cell in cells)
        {
            positions.Add(cell.position);
            Debug.Log(cell);
        }
        return positions;
    }
    public void RemoveAmountOfItem(ItemData item, int amount)
    {
        totalItems[item] -= amount;
    }
    public int CheckAmount(ItemData type)
    {
        return totalItems[type];
    }
    [ContextMenu("CheckInv")]
    void CheckInv()
    {
        foreach (KeyValuePair<ItemData, int> item in totalItems)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }
    }
}
