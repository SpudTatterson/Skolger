using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.buildingSelection.gameObject.SetActive(true);
        EnableButtons();

        BuildingSelectionMenu selectionMenu = UIManager.Instance.buildingSelection;


        selectionMenu.buildingName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";
    }

    public void EnableButtons()
    {
        UIManager.Instance.SetAllActionButtonsInactive();
        UIManager.Instance.deconstructButton.SetActive(true);
    }
}
