using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ColonistInventory : MonoBehaviour, IContainer<InventoryItem>
{
    [field: SerializeField] public InventoryItem[] Items { get; private set; }
    public int InventorySlots { get; private set; } = 1;
    Queue<int> emptySlots = new();

    void Awake()
    {
        Items = new InventoryItem[InventorySlots];
        for (int i = 0; i < Items.Length; i++)
        {
            emptySlots.Enqueue(i);
        }
    }

    public bool HasItem(ItemData itemData, int amount, out int? itemIndex)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (itemData != null && Items[i] != null && itemData == Items[i].itemData && amount <= Items[i].amount)
            {
                itemIndex = i;
                return true;
            }
        }
        itemIndex = null;
        return false;
    }
    public bool IsEmpty()
    {
        foreach (InventoryItem item in Items)
        {
            if (!InventoryItem.CheckIfItemIsNull(item)) return false;
        }
        return true;
    }

    public bool HasSpace()
    {
        if (emptySlots.Count > 0)
            return true;
        return false;
    }

    public InventoryItem TakeItemOut(ItemData itemData, int amount)
    {
        if (HasItem(itemData, amount, out int? itemIndex))
        {
            Items[(int)itemIndex].UpdateAmount(-amount);
            return new InventoryItem(itemData, amount);
        }
        return null;
    }
    public InventoryItem TakeItemOut(int ItemIndex)
    {
        InventoryItem item = new InventoryItem(Items[ItemIndex]);
        Items[ItemIndex].UpdateAmount(-Items[ItemIndex].amount);
        return item;
    }

    public void PutItemIn(InventoryItem item)
    {
        item.OnDestroy += HandleItemDestruction;
        int invIndex = emptySlots.Dequeue();
        item.UpdateOccupiedInventorySlot(invIndex);
        Items[invIndex] = item;
    }

    void HandleItemDestruction(InventoryItem item)
    {
        item.OnDestroy -= HandleItemDestruction;
        Items[item.currentInventorySlot] = null;
        emptySlots.Enqueue(item.currentInventorySlot);
    }
}