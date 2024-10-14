using System;
using Skolger.UI.ToolTip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.UI.Inventory
{

    public class InventoryUI : MonoBehaviour
    {
        public SerializableDictionary<ItemType, SerializableDictionary<ItemData, InventoryUIItem>> items = new();
        public SerializableDictionary<ItemType, ItemTypeUI> itemTypeParents = new();

        void OnEnable()
        {
            InventoryManager.Instance.OnInventoryUpdated += UpdateUI;
        }
        void OnDisable()
        {
            InventoryManager.Instance.OnInventoryUpdated -= UpdateUI;
        }
        void UpdateUI(ItemData item, int amount)
        {
            if (items.ContainsKey(item.itemType) && items[item.itemType].ContainsKey(item))
            {
                // update existing item
                var uiItem = items[item.itemType][item];
                uiItem.UpdateAmount(amount);
            }
            else
            {
                if (!items.ContainsKey(item.itemType))
                {
                    // create new type container
                    items[item.itemType] = new SerializableDictionary<ItemData, InventoryUIItem>();
                    Transform newType = Instantiate(UIManager.Instance.itemTypeGroupPrefab, UIManager.Instance.inventoryPanel).transform;
                    newType.name = item.itemType.ToString();
                    ItemTypeUI itemType = newType.GetComponent<ItemTypeUI>();
                    itemType.text.text = item.itemType.ToString();
                    itemType.dropDown.SetLayoutToRebuild(UIManager.Instance.inventoryPanel as RectTransform);
                    itemTypeParents[item.itemType] = itemType;
                }
                // create new item
                items[item.itemType][item] = CreateNewUIItem(item, amount);

            }
        }

        private InventoryUIItem CreateNewUIItem(ItemData item, int amount)
        {
            GameObject newItemGO = Instantiate(UIManager.Instance.defaultInventoryUIPrefab, itemTypeParents[item.itemType].transform);
            TextMeshProUGUI newItemText = newItemGO.GetComponent<TextMeshProUGUI>();
            Image icon = newItemGO.GetComponentInChildren<Image>();
            ToolTipOnHover toolTipper = newItemGO.GetComponentInChildren<ToolTipOnHover>();

            icon.sprite = item.icon;

            InventoryUIItem uiItem = new InventoryUIItem(newItemText, amount, item.itemName, icon, toolTipper);
            return uiItem;
        }
    }


    [Serializable]
    public class InventoryUIItem
    {
        public TextMeshProUGUI textComponent;
        public Image icon;
        int amount;
        string name;
        public ToolTipOnHover toolTipper;

        public InventoryUIItem(TextMeshProUGUI textComponent, int amount, string name, Image icon, ToolTipOnHover toolTipper)
        {
            this.textComponent = textComponent;
            this.amount = amount;
            this.name = name;
            this.icon = icon;
            this.toolTipper = toolTipper;
            UpdatedUI();
        }
        public void UpdateAmount(int amountToAdd)
        {
            amount += amountToAdd;
            UpdatedUI();
        }

        public void UpdatedUI()
        {
            textComponent.text = $"{amount} x {name}";
            toolTipper.SetHoverText(textComponent.text);
        }
    }
}