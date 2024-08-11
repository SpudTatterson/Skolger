using UnityEngine;

public interface IContainer<T> where T : IItem
{
    T[] Items { get; }
    int InventorySlots { get; }
    bool HasItem(ItemData itemData, int amount, out int? itemIndex);
    bool HasSpace();
    bool IsEmpty();
    T TakeItemOut(ItemData itemData, int amount);
    void PutItemIn(T item);
}