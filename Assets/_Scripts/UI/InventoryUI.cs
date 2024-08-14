using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public SerializableDictionary<ItemType, SerializableDictionary<ItemData, InventoryUIItem>> items = new();
    public SerializableDictionary<ItemType, Transform> itemTypeParents = new();

    void OnEnable()
    {
        InventoryManager.instance.OnInventoryUpdated += UpdateUI;
    }
    void OnDisable()
    {
        InventoryManager.instance.OnInventoryUpdated -= UpdateUI;
    }
    void UpdateUI(ItemData item, int amount)
    {
        if (items.ContainsKey(item.itemType))
        {
            // update existing item
            var uiItem = items[item.itemType][item];
            uiItem.amount += amount;
            uiItem.textComponent.text = $"{uiItem.amount} x {item.itemName}";
        }
        else
        {
            if (!items.ContainsKey(item.itemType))
            {
                // create new type container
                items[item.itemType] = new SerializableDictionary<ItemData, InventoryUIItem>();
                itemTypeParents[item.itemType] = Instantiate(UIManager.Instance.itemTypeGroupPrefab, UIManager.Instance.inventoryPanel).transform;
                itemTypeParents[item.itemType].name = item.itemType.ToString();
                itemTypeParents[item.itemType].GetComponent<TextMeshProUGUI>().text = item.itemType.ToString();
            }
            // create new item
            GameObject newItemGO = Instantiate(UIManager.Instance.defaultInventoryUIPrefab, itemTypeParents[item.itemType]);
            TextMeshProUGUI newItemText = newItemGO.GetComponent<TextMeshProUGUI>();
            Image icon = newItemGO.GetComponentInChildren<Image>();
            newItemText.text = $"{amount} x {item.itemName}";
            icon.sprite = item.icon;

            items[item.itemType][item] = new InventoryUIItem(newItemText, amount, icon);

        }
    }
}


[Serializable]
public class InventoryUIItem
{
    public TextMeshProUGUI textComponent;
    public Image icon;
    public int amount;

    public InventoryUIItem(TextMeshProUGUI textComponent, int amount, Image icon)
    {
        this.textComponent = textComponent;
        this.amount = amount;
        this.icon = icon;
    }
}