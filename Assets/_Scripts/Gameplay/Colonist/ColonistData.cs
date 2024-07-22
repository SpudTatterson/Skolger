using UnityEngine;

public class ColonistData : MonoBehaviour, IHungerable, IContainer<InventoryItem>
{
    const float Max_Belly_Capacity = 100f;
    [field:SerializeField]public float HungerThreshold { get; private set; } = 40;
    [field:SerializeField]public float HungerLevel { get; private set; } = 50;
    [field:SerializeField]public float HungerGainSpeed {get; private set; } = 1;
    InventoryItem heldItem;

    public void Eat(IEdible edible)
    {
        HungerLevel += edible.FoodValue;
        Mathf.Clamp(HungerLevel, 0, Max_Belly_Capacity);
        //Destroy(((MonoBehaviour)edible).gameObject);
    }

    public void GetHungry(float hunger)
    {
        HungerLevel -= HungerGainSpeed * hunger;
        Mathf.Clamp(HungerLevel, 0 , Max_Belly_Capacity);
    }

    public bool IsHungry()
    {
        if (HungerLevel < HungerThreshold) return true;
        return false;
    }
    public bool HasItem(ItemData itemData, int amount)
    {
        if (itemData == heldItem.itemData && amount <= heldItem.amount) return true;
        return false;
    }
    public bool HasSpace()
    {
        if (heldItem == null || heldItem.NullCheck()) return true;
        else return false;
    }

    public InventoryItem TakeItemOut(ItemData itemData, int amount)
    {
        if (HasItem(itemData, amount))
        {
            heldItem.UpdateAmount(-amount);
            return new InventoryItem(itemData, amount);
        }
        return null;
    }

    public void PutItemIn(InventoryItem item)
    {
        item.OnDestroy += HandleItemDestruction;
        heldItem = item;
    }

    void HandleItemDestruction(InventoryItem item)
    {
        item.OnDestroy -= HandleItemDestruction;
        heldItem = null;
    }

    void Update()
    {
        GetHungry(Time.deltaTime);
    }
}