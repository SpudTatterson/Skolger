using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; private set; }
    List<ISelectable> currentSelected = new List<ISelectable>();
    SelectionType selectionType;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More then one SelectionManager");
            Destroy(this);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectAll();
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50, LayerManager.instance.SelectableLayerMask) &&
            hit.transform.TryGetComponent(out ISelectable selectable) &&
            Input.GetKeyDown(KeyCode.Mouse0) && !currentSelected.Contains(selectable))
        {
            selectable.OnSelect();
            UIManager.instance.selectionPanel.SetActive(true);
            Debug.Log("selected " + currentSelected.Count);

        }

    }

    public void SetSelectionType(SelectionType selectionType)
    {

        if (this.selectionType != selectionType)
        {
            foreach (ISelectable selectable in currentSelected)
            {
                selectable.OnDeselect();
            }
        }

        if (selectionType == SelectionType.Item)
        {
            SelectedItem();
        }
        else if (selectionType == SelectionType.Constructable)
        {
            SelectedConstructable();
        }
        this.selectionType = selectionType;
    }

    void SelectedConstructable()
    {
        throw new NotImplementedException();
    }

    void SelectedItem()
    {
        UIManager.instance.itemSelection.gameObject.SetActive(true);
        if (currentSelected.Count > 1)
        {
            // show multiple selection menu
        }
        else
        {
            ItemObject selectedItem = currentSelected[0].GetGameObject().GetComponent<ItemObject>();
            ItemSelectionMenu selectionMenu = UIManager.instance.itemSelection;

            selectionMenu.itemName.text = $"Item Name: {selectedItem.itemData.name}";
            selectionMenu.amount.text = $"Amount: {selectedItem.amount}";
            selectionMenu.stackSize.text = $"Stack Size: {selectedItem.itemData.stackSize}";
        }
    }
    public void AddToCurrentSelected(ISelectable selectable)
    {
        currentSelected.Add(selectable);
    }
    public void RemoveFromCurrentSelected(ISelectable selectable)
    {
        currentSelected.Remove(selectable);
    }
    void DeselectAll()
    {
        List<ISelectable> selectedCopy = new List<ISelectable>(currentSelected);

        foreach (ISelectable selectable in selectedCopy)
        {
            selectable.OnDeselect();
        }
        currentSelected.Clear();
        UIManager.instance.selectionPanel.SetActive(false);
    }
}
