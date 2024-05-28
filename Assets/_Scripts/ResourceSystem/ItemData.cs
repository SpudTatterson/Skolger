using UnityEngine;
using UnityEngine.UI;

public abstract class ItemData : ScriptableObject 
{
    public string itemName;
    public ItemType itemType;
    public int stackSize;
    public GameObject visual;
    public Image icon;
}