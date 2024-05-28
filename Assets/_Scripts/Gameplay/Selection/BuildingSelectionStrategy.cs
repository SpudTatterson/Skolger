using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
          UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.buildingSelection.gameObject.SetActive(true);
        UIManager.instance.EnableBuildingButtons();

        //BuildingObject building = selectedItems[0].GetGameObject().GetComponent<BuildingObject>();

        BuildingSelectionMenu selectionMenu = UIManager.instance.buildingSelection;

        
        selectionMenu.buildingName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";
    }
}
