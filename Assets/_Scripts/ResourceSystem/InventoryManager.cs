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
    public Stockpile GetStockpileWithEmptySpace(out Cell cell)
    {
        foreach (Stockpile stockpile in stockpiles)
        {
            if(stockpile.GetEmptyCell(out cell))
                return stockpile;
        }
        cell = null;
        return null;
    }
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
    public InventoryItem TakeItem(ItemCost ItemCost)
    {
        Stockpile stockpile = GetStockpileWithItem(ItemCost.item, ItemCost.cost);
        return stockpile.TakeItem(ItemCost.item, ItemCost.cost);
       
    }
    // public List<ItemObject> TakeItems(List<ItemCost> ItemCosts)
    // {
    //     List<ItemObject> itemToTake = new List<ItemObject>();
    //     foreach (ItemCost cost in ItemCosts)
    //     {
    //         List<ItemObject> items = TakeItem(cost);
    //         foreach (ItemObject item in items)
    //         {
    //             itemToTake.Add(item);
    //         }
    //     }
    //     return itemToTake;
    // }
    public Cell GetItemLocation(ItemData itemData, int cost)
    {
        Cell cell = GetStockpileWithItem(itemData, cost).GetItemCell(itemData, cost);
        return cell;
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
