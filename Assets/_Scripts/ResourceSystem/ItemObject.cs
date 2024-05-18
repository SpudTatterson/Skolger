using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;
    [SerializeField] int initialAmount;
    public int amount { get; private set; }
    string itemName;
    int stackSize;
    ItemType itemType;
    public GameObject visualGO { get; private set; }
    [SerializeField] bool doManualInitialized = false;
    [SerializeField] bool inStockpile = false;
    Stockpile currentStockpile;

    void Awake()
    {
        if(doManualInitialized)
            Initialize(itemData);
    }

    public void Initialize(ItemData itemData, int amount = 0, GameObject visualGO = null, bool inStockpile = false, Stockpile stockpile= null)
    {
        this.itemData = itemData;

        this.amount = amount;
        itemName = itemData.name;
        stackSize = itemData.stackSize;
        itemType = itemData.itemType;
        this.visualGO = visualGO;
        this.inStockpile = inStockpile;
        currentStockpile = stockpile;

        UpdateAmount(initialAmount);
    }

    public bool UpdateAmount(int amount)
    {
        if (this.amount + amount < stackSize)
        {
            this.amount += amount;
            return true;
        }
        return false;
    }

    public static ItemObject MakeInstance(ItemData itemData, int amount = 0, Transform parent = null, bool inStockpile = false, Stockpile stockpile= null)
    {
        GameObject itemGO = new GameObject(itemData.name);

        itemGO.transform.parent = parent;
        itemGO.layer = LayerManager.instance.itemLayer;
        itemGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        itemGO.AddComponent<BoxCollider>();

        ItemObject item = itemGO.AddComponent<ItemObject>();
        item.Initialize(itemData, amount, parent.gameObject, inStockpile, stockpile);

        return item;
    }

    public bool CheckIfInStockpile(out Stockpile stockpile)
    {
        stockpile = currentStockpile;
        return inStockpile;
    }
}
