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
        if (doManualInitialized)
            Initialize(itemData);
    }

    public void Initialize(ItemData itemData, int amount = 0, GameObject visualGO = null, bool inStockpile = false, Stockpile stockpile = null)
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

    public static ItemObject MakeInstance(ItemData itemData, int amount, Vector3 position, out GameObject visualGO, Transform parent = null, bool inStockpile = false, Stockpile stockpile = null)
    {
        visualGO = Instantiate(itemData.visual, position, Quaternion.identity);

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
