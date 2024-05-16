using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCost 
{
    public ItemData item;
    public int cost;

    public ItemCost(ItemData itemData, int cost)
    {
        this.item = itemData;
        this.cost = cost;
    } 

    public override string ToString()
    {
        return "Type: " +  item +" Cost: " + cost.ToString();
    }
}
