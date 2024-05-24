using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [Header("Settings")]
    public ItemData itemData;
    [SerializeField] int initialAmount;
    int stackSize;
    [SerializeField] bool doManualInitialized = false;
    [SerializeField] bool inStockpile = false;

    public int amount { get; private set; }

    [Header("References")]
    Stockpile currentStockpile;
    public GameObject visualGO { get; private set; }
    public Cell occupiedCell { get; private set; }

    void Start()
    {
        if (doManualInitialized)
            Initialize(itemData, amount);
    }

    public void Initialize(ItemData itemData, int amount, GameObject visualGO = null, bool inStockpile = false, Stockpile stockpile = null)
    {
        this.itemData = itemData;

        this.amount = amount;
        stackSize = itemData.stackSize;
        this.visualGO = visualGO;
        this.inStockpile = inStockpile;
        currentStockpile = stockpile;
        occupiedCell = GridManager.instance.GetGridFromPosition(transform.position).GetCellFromPosition(transform.position);

        UpdateAmount(initialAmount);
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
    public static ItemObject MakeInstance(ItemData itemData, int amount, Vector3 position, Transform parent = null, bool inStockpile = false, Stockpile stockpile = null)
    {
        GameObject visualGO = Instantiate(itemData.visual, position, Quaternion.identity);

        visualGO.AddComponent<BoxCollider>();

        ItemObject item = visualGO.AddComponent<ItemObject>();
        item.Initialize(itemData, amount, visualGO, inStockpile, stockpile);
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
}
