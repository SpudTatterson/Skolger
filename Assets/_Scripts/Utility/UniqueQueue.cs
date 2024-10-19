using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UniqueQueue<T>
{
    [SerializeField] List<T> items = new List<T>();
    private HashSet<T> itemSet = new HashSet<T>();

    // Add item to the end if it doesn't already exist
    public bool Enqueue(T item)
    {
        if (itemSet.Contains(item))
            return false; // Item is already in the list, no duplicates allowed

        items.Add(item);
        itemSet.Add(item);
        return true;
    }

    // Remove the item at the front of the list (FIFO behavior)
    public T Dequeue()
    {
        if (items.Count == 0)
            throw new System.InvalidOperationException("The list is empty.");

        T item = items[0];
        items.RemoveAt(0);
        itemSet.Remove(item);
        return item;
    }

    // Remove a specific item from the list
    public bool Remove(T item)
    {
        if (!itemSet.Contains(item))
            return false;

        items.Remove(item);
        itemSet.Remove(item);
        return true;
    }

    // Get the count of items in the list
    public int Count => items.Count;

    // Check if the list contains a specific item
    public bool Contains(T item)
    {
        return itemSet.Contains(item);
    }

    // Indexer to access elements by index
    public T this[int index]
    {
        get => items[index];
        set
        {
            if (!itemSet.Contains(value))
            {
                itemSet.Remove(items[index]);
                items[index] = value;
                itemSet.Add(value);
            }
            else
            {
                throw new System.InvalidOperationException("Cannot assign duplicate value.");
            }
        }
    }
}
