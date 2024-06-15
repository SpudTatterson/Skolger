using System;
using UnityEngine;
using NaughtyAttributes;

public class ItemObject : MonoBehaviour, ISelectable, IAllowable
{
    [Header("Settings")]
    public ItemData itemData;
    [SerializeField] int initialAmount;
    int stackSize;
    [SerializeField] bool doManualInitialized = false;
    [SerializeField] bool inStockpile = false;
    [SerializeField] bool allowedOnInit = true;

    [field: SerializeField, ReadOnly] public int amount { get; private set; }

    [Header("References")]
    Stockpile currentStockpile;
    public GameObject visualGO { get; private set; }
    public Cell occupiedCell { get; private set; }
    public bool allowed { get; private set; }




    void Start()
    {
        if (doManualInitialized)
            Initialize(itemData, initialAmount, allowedOnInit);
    }

    public void Initialize(ItemData itemData, int amount, bool allowed = true, GameObject visualGO = null, bool inStockpile = false, Stockpile stockpile = null)
    {
        this.itemData = itemData;
        stackSize = itemData.stackSize;
        if (!UpdateAmount(amount))
        {
            Debug.Log("tried to initiate with invalid amount");
            Destroy(gameObject);
            return;
        }
        this.visualGO = visualGO;
        this.inStockpile = inStockpile;
        currentStockpile = stockpile;
        if (allowed) OnAllow();
        else OnDisallow();
        if (GridManager.instance.GetCellFromPosition(transform.position) == null) occupiedCell = null;
        else
        {
            occupiedCell = GridManager.instance.GetCellFromPosition(transform.position);
            occupiedCell.inUse = true;
        }
    }

    public bool UpdateAmount(int amount)
    {
        int newAmount = this.amount + amount;

        if (newAmount >= 0 && newAmount <= stackSize)
        {
            this.amount = newAmount;
            if (this.amount == 0)
            {
                if (inStockpile)
                    currentStockpile.RemoveItem(this);
                else
                {
                    Debug.Log("destroying object");
                    Destroy(this.gameObject);
                }

            }
            if (this.amount < 0) Debug.LogWarning("item amount less then 0 something went wrong" + transform.name);
            return true;
        }
        return false;
    }

    public ItemObject SplitItem(int amount, Vector3 position, Transform parent = null)
    {
        // remove needed amount into new time and keep the rest return new item
        if (amount > this.amount)
        {
            Debug.Log("tried taking amount larger then amount in item, split failed");
            return null;
        }
        ItemObject newItem = MakeInstance(this.itemData, amount, position, parent);
        UpdateAmount(-amount);


        return newItem;
    }
    public int MergeItem(ItemObject item)
    {
        // take amount into this item and return excess
        int maxIntake = stackSize - amount;
        int takeAmount = Mathf.Min(maxIntake, item.amount);

        this.UpdateAmount(takeAmount);
        item.UpdateAmount(-takeAmount);

        return takeAmount;
    }
    public static ItemObject MakeInstance(ItemData itemData, int amount, Vector3 position, bool allowed = true, Transform parent = null, bool inStockpile = false, Stockpile stockpile = null)
    {
        GameObject visualGO = Instantiate(itemData.visual, position, Quaternion.identity);

        visualGO.AddComponent<BoxCollider>();

        ItemObject item = visualGO.AddComponent<ItemObject>();
        item.Initialize(itemData, amount, allowed, visualGO, inStockpile, stockpile);
        item.transform.position = position;

        visualGO.transform.parent = parent;
        visualGO.layer = LayerManager.instance.itemLayer;

        return item;
    }


    public bool CheckIfInStockpile(out Stockpile stockpile)
    {
        stockpile = currentStockpile;
        return inStockpile;
    }
    public void RemoveFromStockpile()
    {
        if (!inStockpile) return;

        inStockpile = false;
        currentStockpile = null;
        transform.parent = null;
        OnAllow();

        InventoryManager.instance.RemoveAmountOfItem(itemData, amount);
    }

    #region Selection

    public SelectionType GetSelectionType()
    {
        return SelectionType.Item;
    }
    public ISelectionStrategy GetSelectionStrategy()
    {
        return new ItemSelectionStrategy();
    }
    public bool HasActiveCancelableAction()
    {
        return false;
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = this.amount;
        return itemData.itemName;
    }

    #endregion

    #region IAllowable 

    public void OnAllow()
    {
        allowed = true;
        if (!inStockpile)
            FindObjectOfType<HaulerTest>().AddToHaulQueue(this);

    }

    public void OnDisallow()
    {
        allowed = false;
        if (!inStockpile)
            FindObjectOfType<HaulerTest>().RemoveFromHaulQueue(this);
        //remove from haul queue
        // visually show that item is disallowed with billboard or something similar
    }
    public bool IsAllowed()
    {
        return allowed;
    }

    #endregion

    void OnDestroy()
    {
        if (!inStockpile)
        {
            occupiedCell.inUse = false;
        }
    }
}
