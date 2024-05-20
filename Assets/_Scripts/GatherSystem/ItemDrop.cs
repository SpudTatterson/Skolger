using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public ItemData itemData;
    public int minDropAmount;
    public int maxDropAmount;
    public ItemDrop(ItemData itemData, int minDropAmount, int maxDropAmount)
    {
        this.itemData = itemData;
        this.minDropAmount = minDropAmount;
        this.maxDropAmount = maxDropAmount;

    }


}
