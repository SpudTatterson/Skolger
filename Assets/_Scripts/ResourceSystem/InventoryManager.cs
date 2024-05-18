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
    public void AddItem(ItemData item, int amount)
    {
        if(totalItems.ContainsKey(item))
            totalItems[item] += amount;
        else
            totalItems.Add(item, amount);
    }

    public bool HasItem(ItemCost materialCost)
    {
        if (!totalItems.ContainsKey(materialCost.item)) return false;
        if (totalItems[materialCost.item] >= materialCost.cost) return true;
        else return false;
    }
    public bool HasItems(List<ItemCost> materialCosts)
    {
        foreach (ItemCost Cost in materialCosts)
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
    public void UseItem(ItemCost materialCost)
    {
        totalItems[materialCost.item] -= materialCost.cost;
    }
    public void UseItems(List<ItemCost> materialCosts)
    {
        foreach (ItemCost cost in materialCosts)
        {
            totalItems[cost.item] -= cost.cost;
        }
    }

    public int CheckAmount(ItemData type)
    {
        return totalItems[type];
    }
    [ContextMenu("CheckInv")]
    void CheckInv()
    {
        foreach (KeyValuePair<ItemData, int> material in totalItems)
        {
            Debug.Log(material.Key + ": " + material.Value);
        }
    }
}
