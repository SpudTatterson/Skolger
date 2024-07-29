using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColonistData : MonoBehaviour, IHungerable, IContainer<InventoryItem>
{
    const float Max_Belly_Capacity = 100f;
    [field: SerializeField] public float HungerThreshold { get; private set; } = 40; // The amount of hungry at which the colonist will drop everything and go eat
    [field: SerializeField, ReadOnly] public float HungerLevel { get; private set; } = 50; // How hungry the colonist current is
    [field: SerializeField] public float HungerGainSpeed { get; private set; } = 1; // Hunger gain per second

    [field: SerializeField] public InventoryItem[] Items { get; private set; }
    public int InventorySlots { get; private set; } = 1;
    Queue<int> emptySlots = new();

    public Sprite faceSprite;
    public int width = 256;
    public int height = 256;

    public event Action<string> OnActivityChanged;
    private string _colonistActivity;
    [HideInInspector] public string colonistName { get; private set; }
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

    void Awake()
    {
        Items = new InventoryItem[InventorySlots];
        for (int i = 0; i < Items.Length; i++)
        {
            emptySlots.Enqueue(i);
        }
        colonistName = SetRandomName();
    }

    void Start()
    {
        faceSprite = CaptureFace();
        UIManager.instance.AddColonistToBoard(colonistName, this);
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
        GetHungry(Time.deltaTime);
    }

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, Mathf.Infinity, ~gameObject.layer))
        {
            DisplayInfo();
        }
    }

    public void DisplayInfo()
    {
        UIManager.instance.ShowColonistWindow(colonistName, colonistActivity);
        UIManager.instance.SetCurrentColonist(this);
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

    public Sprite CaptureFace()
    {
        RenderTexture renderTexture = new RenderTexture(width, height, 24);

        GameObject faceHeight = Instantiate(new GameObject(), gameObject.transform);
        GameObject cameraObject = Instantiate(new GameObject(), gameObject.transform);
        Camera captureCamera = cameraObject.AddComponent<Camera>();

        captureCamera.targetTexture = renderTexture;
        captureCamera.orthographic = false;
        captureCamera.fieldOfView = 60;
        captureCamera.clearFlags = CameraClearFlags.SolidColor;
        captureCamera.backgroundColor = Color.clear;
        captureCamera.farClipPlane = 2.5f;

        faceHeight.transform.position += new Vector3(0, 1.75f, 0);
        captureCamera.transform.position += gameObject.transform.TransformDirection(new Vector3(0,1.75f,1.15f));
        captureCamera.transform.LookAt(faceHeight.transform.position);

        RenderTexture.active = renderTexture;
        captureCamera.Render();

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        RenderTexture.active = null;
        Destroy(cameraObject);
        Destroy(renderTexture);
        Destroy(faceHeight);

        return sprite;
    }
}