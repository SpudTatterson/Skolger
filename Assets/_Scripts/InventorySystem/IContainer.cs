using UnityEngine;

public interface IContainer<T> where T : IItem
{
    bool HasItem(ItemData itemData, int amount);
    bool HasSpace();
    T TakeItemOut(ItemData itemData, int amount);
    void PutItemIn(T item);
}