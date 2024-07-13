using UnityEngine;

public interface IItem
{
    public ItemData itemData { get; }
    public int amount { get; }
    public bool UpdateAmount(int amount);
}