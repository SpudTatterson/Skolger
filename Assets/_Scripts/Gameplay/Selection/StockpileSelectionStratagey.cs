using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StockpileSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.stockpileSelection.gameObject.SetActive(true);
        EnableButtons();

        UIManager.instance.stockpileSelection.stockpileName.text = selectedItems[0].GetMultipleSelectionString(out _);
    }

    public void EnableButtons()
    {
        UIManager.instance.SetAllActionButtonsInactive();

        UIManager.instance.deconstructButton.SetActive(true);
        UIManager.instance.growZoneButton.SetActive(true);
        UIManager.instance.shrinkZoneButton.SetActive(true);
    }
}