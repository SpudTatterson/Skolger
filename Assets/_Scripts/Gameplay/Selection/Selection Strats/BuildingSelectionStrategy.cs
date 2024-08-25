using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.buildingSelection.gameObject.SetActive(true);
        EnableButtons();

        BuildingSelectionMenu selectionMenu = UIManager.instance.buildingSelection;


        selectionMenu.buildingName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";
    }

    public void EnableButtons()
    {
        UIManager.instance.SetAllActionButtonsInactive();
        UIManager.instance.deconstructButton.SetActive(true);
    }
}
