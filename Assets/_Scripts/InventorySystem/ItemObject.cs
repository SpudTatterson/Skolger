using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ItemObject : MonoBehaviour, IItem, ISelectable, IAllowable, ICellOccupier
{
    [field: Header("Settings")]
    [field: SerializeField, InlineEditor] public ItemData itemData { get; set; }
    [SerializeField] int initialAmount;
    int stackSize;
    [SerializeField] bool doManualInitialized = false;
    [SerializeField] bool inStockpile = false;
    [SerializeField] bool allowedOnInit = true;

    [field: SerializeField, ReadOnly] public int amount { get; private set; }

    [Header("References")]
    [SerializeField, ReadOnly] Stockpile currentStockpile;
    [SerializeField, ReadOnly] public GameObject visualGO { get; private set; }
    [SerializeField, ReadOnly] BillBoard forbiddenBillboard;
    [SerializeField, ReadOnly] Outline outline;

    public Cell cornerCell { get; private set; }
    public bool allowed { get; private set; }
    public bool IsSelected { get; private set; }




    void Start()
    {
        if (doManualInitialized)
            Initialize(itemData, initialAmount, allowedOnInit);
    }

    public static ItemObject MakeInstance(ItemData itemData, int amount, Vector3 position, bool allowed = true, Transform parent = null, bool inStockpile = false, Stockpile stockpile = null)
    {
        GameObject mainGO = new GameObject(itemData.name);

        GameObject visualGO = Instantiate(itemData.visual, Vector3.zero, Quaternion.identity, mainGO.transform);
        if (!inStockpile) visualGO.transform.rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 180), 0));

        mainGO.AddComponent<BoxCollider>();

        ItemObject item = mainGO.AddComponent<ItemObject>();

        mainGO.transform.parent = parent;
        item.transform.position = position;

        item.Initialize(itemData, amount, allowed, mainGO, inStockpile, stockpile);

        mainGO.layer = LayerManager.Instance.itemLayer;

        return item;
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
        if (inStockpile)
        {
            currentStockpile = stockpile;
            transform.localPosition = VectorUtility.FlattenVector(transform.localPosition);
        }

        forbiddenBillboard = GetComponentInChildren<BillBoard>(true);
        outline = GetComponentInChildren<Outline>(true);
        if (outline == null)
            outline = visualGO.AddComponent<Outline>();

        if (allowed) OnAllow();
        else OnDisallow();
        if (GridManager.Instance.GetCellFromPosition(transform.position) == null) cornerCell = null;
        else
        {
            cornerCell = GridManager.Instance.GetCellFromPosition(transform.position);
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
                Destroy(this.gameObject);
            }
            if (this.amount < 0) Debug.LogWarning("item amount less then 0 something went wrong" + transform.name);
            return true;
        }
        return false;
    }

    public ItemObject SplitItem(int amount, Vector3 position, Transform parent = null)
    {
        if (amount > this.amount)
        {
            Debug.Log("tried taking amount larger then amount in item, split failed");
            return null;
        }
        ItemObject newItem = MakeInstance(this.itemData, amount, position, parent);
        UpdateAmount(-amount);


        return newItem;
    }
    public int MergeItem(IItem item)
    {
        // take amount into this item and return excess
        int maxIntake = stackSize - amount;
        int takeAmount = Mathf.Min(maxIntake, item.amount);

        this.UpdateAmount(takeAmount);
        item.UpdateAmount(-takeAmount);

        return takeAmount;
    }

    public InventoryItem PickUp()
    {
        Destroy(gameObject);
        if (itemData.itemType == ItemType.Edible)
        {
            EdibleInventoryItem edibleItem = new EdibleInventoryItem((EdibleData)itemData, amount);
            return edibleItem;
        }
        InventoryItem item = new InventoryItem(itemData, amount);
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
        transform.SetParent(null);
        OnAllow();
        OnOccupy();

        InventoryManager.Instance.RemoveAmountOfItem(itemData, amount);
    }

    #region Selection

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.AddToCurrentSelected(this);
        IsSelected = true;

        outline?.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.RemoveFromCurrentSelected(this);
        if (IsSelected)
            manager.UpdateSelection();

        outline?.Disable();
        IsSelected = false;
    }

    public void OnHover()
    {
        outline?.Enable();
    }

    public void OnHoverEnd()
    {
        outline?.Disable();
    }
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
        //Debug.Log("allowed");
        allowed = true;
        if (!inStockpile)
            TaskManager.Instance.AddToHaulQueue(this);

        forbiddenBillboard?.gameObject.SetActive(false);

    }

    public void OnDisallow()
    {
        allowed = false;
        if (!inStockpile)
            TaskManager.Instance.RemoveFromHaulQueue(this);
        //remove from haul queue
        // visually show that item is disallowed with billboard or something similar
        forbiddenBillboard?.gameObject.SetActive(true);
    }
    public bool IsAllowed()
    {
        return allowed;
    }

    #endregion

    #region ICellOccupier

    public void GetOccupiedCells()
    {
        cornerCell = GridManager.Instance.GetCellFromPosition(transform.position);
    }
    public void OnOccupy()
    {
        cornerCell.inUse = true;
    }

    public void OnRelease()
    {
        cornerCell.inUse = false;
    }

    #endregion

    void OnDisable()
    {
        if (!inStockpile)
        {
            OnRelease();
        }
        OnDeselect();
    }
    void OnEnable()
    {
        GetOccupiedCells();
        OnOccupy();
    }
    void OnDestroy()
    {
        if (inStockpile)
            currentStockpile.RemoveItem(this);
    }

}
