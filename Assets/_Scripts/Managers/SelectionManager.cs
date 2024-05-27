using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; private set; }
    List<ISelectable> currentSelected = new List<ISelectable>();
    Dictionary<string, int> selectedNamesAndAmounts = new Dictionary<string, int>();
    List<TextMeshProUGUI> multipleSelectionTexts = new List<TextMeshProUGUI>();
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
        HandleDeselectionInput();

        HandleSelection();
    }

    #region Selection Logic

    void HandleSelection()
    {
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

    void HandleDeselectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectAll();
        }
    }

    #endregion

    #region Public methods

    public void SetSelectionType(SelectionType selectionType)
    {
        if (currentSelected.Count > 1)
        {
            SelectedMultiple();
            this.selectionType = SelectionType.Multiple;
            return;
        }

        if (selectionType == SelectionType.Item)
        {
            SelectedItem();
        }
        else if (selectionType == SelectionType.Constructable)
        {
            SelectedConstructable();
        }
        else if (selectionType == SelectionType.Harvestable)
        {
            SelectedHarvestable();
        }
        else if (selectionType == SelectionType.Colonist)
        {
            SelectedColonist();
        }
        this.selectionType = selectionType;
    }
    public void AddToCurrentSelected(ISelectable selectable)
    {
        currentSelected.Add(selectable);
    }
    public void RemoveFromCurrentSelected(ISelectable selectable)
    {
        currentSelected.Remove(selectable);
    }

    #endregion

    #region Selectable Types

    void SelectedMultiple()
    {
        SetAllSelectionUIInactive();
        UIManager.instance.multipleSelection.SetActive(true);

        Transform scrollViewContent = UIManager.instance.multipleSelectionContent;
        ClearMultipleSelectionTexts();
        selectedNamesAndAmounts.Clear();
        SelectionType type = currentSelected[0].GetSelectionType();
        bool allSelectedOfSameType = true;
        foreach (ISelectable selectable in currentSelected)
        {
            string selectionString = selectable.GetMultipleSelectionString(out int amount);
            if (selectedNamesAndAmounts.ContainsKey(selectionString))
                selectedNamesAndAmounts[selectionString] += amount;
            else
                selectedNamesAndAmounts.Add(selectionString, amount);

            bool sameType = selectable.GetSelectionType() == type;
            if (!sameType)
                allSelectedOfSameType = false;
        }

        foreach (KeyValuePair<string, int> pair in selectedNamesAndAmounts)
        {

            TextMeshProUGUI text = Instantiate(UIManager.instance.defaultTextAsset, scrollViewContent);
            text.text = $"{pair.Key} x {pair.Value}";
            multipleSelectionTexts.Add(text);
        }

        if (allSelectedOfSameType)
            EnableButtons(type);
        else
            SetAllActionButtonsInactive();
    }

    void SelectedConstructable()
    {
        throw new NotImplementedException();
    }

    void SelectedItem()
    {
        SetAllSelectionUIInactive();
        UIManager.instance.itemSelection.gameObject.SetActive(true);
        EnableItemButtons();

        ItemObject selectedItem = currentSelected[0].GetGameObject().GetComponent<ItemObject>();
        ItemSelectionMenu selectionMenu = UIManager.instance.itemSelection;

        selectionMenu.itemName.text = $"Item Name: {selectedItem.itemData.name}";
        selectionMenu.amount.text = $"Amount: {selectedItem.amount}";
        selectionMenu.stackSize.text = $"Stack Size: {selectedItem.itemData.stackSize}";
    }

    void SelectedColonist()
    {
        throw new NotImplementedException();
    }

    void SelectedHarvestable()
    {
        SetAllSelectionUIInactive();
        UIManager.instance.harvestableSelection.gameObject.SetActive(true);
        EnableHarvestableButtons();

        IHarvestable harvestable = currentSelected[0].GetGameObject().GetComponent<IHarvestable>();
        List<ItemDrop> drops = harvestable.GetItemDrops();

        HarvestableSelectionMenu selectionMenu = UIManager.instance.harvestableSelection;

        selectionMenu.SetDrops(drops);
        selectionMenu.harvestableName.text = $"Name: {currentSelected[0].GetMultipleSelectionString(out _)}";

        CheckForCancelableAction();
    }

    #endregion

    #region Buttons

    void EnableButtons(SelectionType type)
    {
        if (type == SelectionType.Colonist)
            EnableColonistButtons();
        else if (type == SelectionType.Harvestable)
            EnableHarvestableButtons();
        else if (type == SelectionType.Item)
            EnableItemButtons();
        else if (type == SelectionType.Constructable)
            EnableConstructableButtons();

        CheckForCancelableAction();
    }

    private void CheckForCancelableAction()
    {
        if (currentSelected[0].HasActiveCancelableAction())
        {
            EnableCancelButton();
        }
    }

    void EnableCancelButton()
    {
        UIManager.instance.cancelButton.SetActive(true);
    }

    void EnableConstructableButtons()
    {
        throw new NotImplementedException();
    }

    void EnableItemButtons()
    {
        SetAllActionButtonsInactive();
        UIManager.instance.allowButton.SetActive(true);
    }

    void EnableHarvestableButtons()
    {
        SetAllActionButtonsInactive();
        UIManager.instance.harvestButton.SetActive(true);
    }

    void EnableColonistButtons()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Button Actions

    public void SetToHarvest()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            selectable.GetGameObject().GetComponent<IHarvestable>().AddToHarvestQueue();
        }
        EnableCancelButton();
        UIManager.instance.harvestButton.SetActive(false);
    }

    public void TryToCancelActions()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            if (selectable.GetGameObject().TryGetComponent<IHarvestable>(out IHarvestable harvestable))
            {
                harvestable.RemoveFromHarvestQueue();
                EnableHarvestableButtons();
                UIManager.instance.harvestButton.SetActive(true);
            }
        }
    }

    #endregion

    #region Cleanup
    void DeselectAll()
    {
        List<ISelectable> selectedCopy = new List<ISelectable>(currentSelected);

        foreach (ISelectable selectable in selectedCopy)
        {
            selectable.OnDeselect();
        }
        selectedNamesAndAmounts.Clear();
        ClearMultipleSelectionTexts();
        currentSelected.Clear();
        SetAllSelectionUIInactive();
        UIManager.instance.selectionPanel.SetActive(false);
    }

    void SetAllSelectionUIInactive()
    {
        UIManager uiManager = UIManager.instance;
        uiManager.itemSelection.gameObject.SetActive(false);
        uiManager.harvestableSelection.gameObject.SetActive(false);
        uiManager.multipleSelection.SetActive(false);
    }
    void SetAllActionButtonsInactive()
    {
        UIManager uiManager = UIManager.instance;
        uiManager.allowButton.SetActive(false);
        uiManager.harvestButton.SetActive(false);
        uiManager.cancelButton.SetActive(false);
    }
    void ClearMultipleSelectionTexts()
    {
        foreach (TextMeshProUGUI text in multipleSelectionTexts)
        {
            Destroy(text.gameObject);
        }
        multipleSelectionTexts.Clear();
    }

    #endregion
}
