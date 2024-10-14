using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleSelectionStrategy : ISelectionStrategy
{
    Dictionary<string, int> selectedNamesAndAmounts = new Dictionary<string, int>();
    Dictionary<string, List<ISelectable>> namesAndSelectables = new Dictionary<string, List<ISelectable>>();
    List<MultipleSelectionText> multipleSelectionTexts = new List<MultipleSelectionText>();
    SelectionType type;
    ISelectable mainSelectable;

    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.multipleSelection.SetActive(true);

        Transform scrollViewContent = UIManager.Instance.multipleSelectionContent;
        ClearMultipleSelectionTexts();
        selectedNamesAndAmounts.Clear();
        namesAndSelectables.Clear();
        type = selectedItems[0].GetSelectionType();
        bool allSelectedOfSameType = true;
        foreach (ISelectable selectable in selectedItems)
        {
            string selectionString = selectable.GetMultipleSelectionString(out int amount);
            if (selectedNamesAndAmounts.ContainsKey(selectionString))
            {
                selectedNamesAndAmounts[selectionString] += amount;
                namesAndSelectables[selectionString].Add(selectable);
            }
            else
            {
                selectedNamesAndAmounts.Add(selectionString, amount);
                namesAndSelectables.Add(selectionString, new List<ISelectable> { selectable });
            }

            bool sameType = selectable.GetSelectionType() == type;
            if (!sameType)
                allSelectedOfSameType = false;
        }

        foreach (KeyValuePair<string, int> pair in selectedNamesAndAmounts)
        {
            MultipleSelectionText text = PoolManager.Instance.GetObject(UIManager.Instance.multipleSelectionTextAsset.gameObject, parent: scrollViewContent).GetComponent<MultipleSelectionText>();
            text.Init($"{pair.Value} x {pair.Key}",namesAndSelectables[pair.Key]);
            multipleSelectionTexts.Add(text);
        }

        if (allSelectedOfSameType)
        {
            mainSelectable = selectedItems[0];
            EnableButtons();
        }
        else
            UIManager.Instance.SetAllActionButtonsInactive();
    }


    void ClearMultipleSelectionTexts()
    {
        foreach (MultipleSelectionText text in multipleSelectionTexts)
        {
            PoolManager.Instance.ReturnObject(UIManager.Instance.multipleSelectionTextAsset.gameObject, text.gameObject);
        }
        multipleSelectionTexts.Clear();
    }

    public void CleanUp()
    {
        ClearMultipleSelectionTexts();
    }
    public void EnableButtons()
    {
        mainSelectable.GetSelectionStrategy().EnableButtons();

        SelectionManager.Instance.CheckForCancelableAction();
        SelectionManager.Instance.CheckForAllowableSelection();
    }
}
