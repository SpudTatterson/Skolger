using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StockpileSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.stockpileSelection.gameObject.SetActive(true);
        EnableButtons();

        UIManager.Instance.stockpileSelection.stockpileName.text = selectedItems[0].GetMultipleSelectionString(out _);
    }

    public void EnableButtons()
    {
        UIManager.Instance.SetAllActionButtonsInactive();

        UIManager.Instance.deconstructButton.SetActive(true);
        UIManager.Instance.growZoneButton.SetActive(true);
        UIManager.Instance.shrinkZoneButton.SetActive(true);
    }
}