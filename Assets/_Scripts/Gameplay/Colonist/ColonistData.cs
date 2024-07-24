using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ColonistData : MonoBehaviour, IHungerable, IContainer<InventoryItem>
{
    const float Max_Belly_Capacity = 100f;
    [field: SerializeField] public float HungerThreshold { get; private set; } = 40; // The amount of hungry at which the colonist will drop everything and go eat
    [field: SerializeField, ReadOnly] public float HungerLevel { get; private set; } = 50; // How hungry the colonist current is
    [field: SerializeField] public float HungerGainSpeed { get; private set; } = 1; // Hunger gain per second

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
    public void Eat(IEdible edible)
    {
        HungerLevel += edible.FoodValue;
        HungerLevel = Mathf.Clamp(HungerLevel, 0, Max_Belly_Capacity);
        //Destroy(((MonoBehaviour)edible).gameObject);
    }

    public void GetHungry(float hunger)
    {
        HungerLevel -= HungerGainSpeed * hunger;
        HungerLevel = Mathf.Clamp(HungerLevel, 0, Max_Belly_Capacity);
    }

    public bool IsHungry()
    {
        if (HungerLevel < HungerThreshold) return true;
        return false;
    }



    public bool HasItem(ItemData itemData, int amount, out int? itemIndex)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (itemData == Items[i].itemData && amount <= Items[i].amount)
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
        item.currentInventorySlot = invIndex;
        Items[invIndex] = item;
    }

    void HandleItemDestruction(InventoryItem item)
    {
        item.OnDestroy -= HandleItemDestruction;
        Items[item.currentInventorySlot] = null;
        emptySlots.Enqueue(item.currentInventorySlot);
    }

    void Update()
    {
        GetHungry(Time.deltaTime);
    }
}