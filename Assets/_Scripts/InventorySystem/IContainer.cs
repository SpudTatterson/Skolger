using UnityEngine;

public interface IContainer<T> where T : IItem
{
    bool HasItem(ItemData itemData, int amount);
    T TakeItem(ItemData itemData, int amount);
    bool AddItem(T item);
}