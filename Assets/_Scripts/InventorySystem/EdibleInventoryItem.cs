using UnityEngine;

public class EdibleInventoryItem : InventoryItem, IEdible
{
    public EdibleData edibleData;
    public EdibleInventoryItem(EdibleData itemData, int amount) : base(itemData, amount)
    {
        edibleData = itemData;
    }

    public float FoodValue => edibleData.FoodValue;
}