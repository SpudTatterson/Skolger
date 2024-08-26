using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColonistData : MonoBehaviour, IContainer<InventoryItem>, ISelectable
{

    const float Max_Belly_Capacity = 100f;
    public HungerManager hungerManager;

    [field: Header("Inventory")]
    [field: SerializeField] public InventoryItem[] Items { get; private set; }
    public int InventorySlots { get; private set; } = 1;
    Queue<int> emptySlots = new();

    [Header("Portrait")]
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    public Sprite faceSprite { get; private set; }

    public event Action<string> OnActivityChanged;
    [HideInInspector] public string colonistName { get; private set; }
    private string _colonistActivity;
    [HideInInspector]
    public string colonistActivity
    {
        get => _colonistActivity;
        set
        {
            if (_colonistActivity != value)
            {
                _colonistActivity = value;
                OnActivityChanged?.Invoke(_colonistActivity);
            }
        }
    }

    [Header("Selection")]
    [SerializeField] Outline outline;
    public bool IsSelected { get; private set; }
    void Awake()
    {
        hungerManager = GetComponent<HungerManager>();
        Items = new InventoryItem[InventorySlots];
        for (int i = 0; i < Items.Length; i++)
        {
            emptySlots.Enqueue(i);
        }
        colonistName = SetRandomName();
    }

    void Start()
    {
        faceSprite = ColonistUtility.CaptureFace(gameObject, 1.75f, new Vector3(0, 1.75f, 1.15f), width, height, 1.5f);
        UIManager.Instance.AddColonistToBoard(colonistName, this);
    }


    public bool HasItem(ItemData itemData, int amount, out int? itemIndex)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (itemData != null && Items[i] != null && itemData == Items[i].itemData && amount <= Items[i].amount)
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
        item.UpdateOccupiedInventorySlot(invIndex);
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
        hungerManager.GetHungry(Time.deltaTime);
    }
    }

    public void DisplayInfo()
    {
        UIManager.Instance.ShowColonistWindow(colonistName, colonistActivity);
        UIManager.Instance.SetCurrentColonist(this);
    }

    string SetRandomName()
    {
        List<string> firstNames = new List<string>
        {
            "Erik",
            "Bjorn",
            "Sigrid",
            "Leif",
            "Astrid",
            "Olaf",
            "Freya",
            "Ivar",
            "Gunnar",
            "Helga",
            "Ragnhild",
            "Sven",
            "Ingrid",
            "Harald",
            "Ragnar"
        };

        List<string> lastNames = new List<string>
        {
            "Halden",
            "Strand",
            "Berg",
            "Fjord",
            "Alfheim",
            "Hamar",
            "Kjell",
            "Vik",
            "Skog",
            "Lothbrok",
            "Dal",
            "Stav",
            "Voll",
            "Ask",
            "Grove",
        };

        int firstName = Random.Range(0, firstNames.Count);
        int lastName = Random.Range(0, lastNames.Count);

        return firstNames[firstName] + " " + lastNames[lastName];
    }

    public void ChangeActivity(string activity)
    {
        colonistActivity = activity;
    }


    #region ISelectable

    public SelectionType GetSelectionType()
    {
        return SelectionType.Colonist;
    }

    public ISelectionStrategy GetSelectionStrategy()
    {
        return new ColonistSelectionStrategy();
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return colonistName;
    }

    public bool HasActiveCancelableAction()
    {
        return false;
    }

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.AddToCurrentSelected(this);
        IsSelected = true;

        outline.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.RemoveFromCurrentSelected(this);
        if (IsSelected)
            manager.UpdateSelection();

        outline.Disable();
        IsSelected = false;
    }

    public void OnHover()
    {
        outline.Enable();
    }

    public void OnHoverEnd()
    {
        outline.Disable();
    }

    #endregion
}