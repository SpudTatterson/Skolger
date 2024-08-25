using System;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class InventoryItem : IItem
{

    [field: SerializeField, InlineEditor] public ItemData itemData { get; private set; }
    [field: SerializeField, ReadOnly] public int amount { get; private set; }

    public int currentInventorySlot { get; private set; }

    public event Action<InventoryItem> OnDestroy;
    public InventoryItem(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        this.amount = amount;
    }
    public InventoryItem(InventoryItem itemToCopy)
    {
        itemData = itemToCopy.itemData;
        amount = itemToCopy.amount;
        
    }

    public bool UpdateAmount(int amount)
    {
        int newAmount = this.amount + amount;

        if (newAmount >= 0 && newAmount <= itemData.stackSize)
        {
            this.amount = newAmount;
            if (this.amount == 0)
            {
                Destroy();
            }
            if (this.amount < 0) Debug.LogWarning("item amount less then 0 something went wrong" + amount);
            return true;
        }
        return false;
    }
    public void UpdateOccupiedInventorySlot(int InventoryIndex)
    {
        currentInventorySlot = InventoryIndex;
    }

    public ItemObject DropItem(Vector3 position)
    {
        return ItemObject.MakeInstance(itemData, amount, position);
    }

    public void Destroy()
    {
        OnDestroy?.Invoke(this);
    }

    public bool NullCheck()
    {
        return itemData == null;
    }

    public static bool CheckIfItemIsNull(InventoryItem item)
    {
        return item == null || item.NullCheck();
    }

    public override string ToString()
    {
        return $"{itemData.itemName} {amount}";
    }
}