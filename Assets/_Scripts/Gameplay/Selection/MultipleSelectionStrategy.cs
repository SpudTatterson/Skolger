using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultipleSelectionStrategy : ISelectionStrategy
{
    Dictionary<string, int> selectedNamesAndAmounts = new Dictionary<string, int>();
    List<TextMeshProUGUI> multipleSelectionTexts = new List<TextMeshProUGUI>();

    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.multipleSelection.SetActive(true);

        Transform scrollViewContent = UIManager.instance.multipleSelectionContent;
        ClearMultipleSelectionTexts();
        selectedNamesAndAmounts.Clear();
        SelectionType type = selectedItems[0].GetSelectionType();
        bool allSelectedOfSameType = true;
        foreach (ISelectable selectable in selectedItems)
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

            TextMeshProUGUI text = MonoBehaviour.Instantiate(UIManager.instance.defaultTextAsset, scrollViewContent);
            text.text = $"{pair.Key} x {pair.Value}";
            multipleSelectionTexts.Add(text);
        }

        if (allSelectedOfSameType)
        {
            if (selectedItems[0].GetGameObject().TryGetComponent(out IAllowable allowable))
                UIManager.instance.EnableButtons(type, allowable.IsAllowed());
            else
                UIManager.instance.EnableButtons(type, false);
        }
        else
            UIManager.instance.SetAllActionButtonsInactive();
    }


    void ClearMultipleSelectionTexts()
    {
        foreach (TextMeshProUGUI text in multipleSelectionTexts)
        {
            MonoBehaviour.Destroy(text.gameObject);
        }
        multipleSelectionTexts.Clear();
    }

    public void CleanUp()
    {
        ClearMultipleSelectionTexts();
    }
}