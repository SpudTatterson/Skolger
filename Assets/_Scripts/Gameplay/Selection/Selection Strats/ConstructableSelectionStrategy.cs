using System.Collections.Generic;

public class ConstructableSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.constructableSelection.gameObject.SetActive(true);
        EnableButtons();

        IConstructable constructable = selectedItems[0] as IConstructable;
        List<ItemCost> costs = constructable.GetAllCosts();

        ConstructableSelectionMenu selectionMenu = UIManager.Instance.constructableSelection;

        selectionMenu.SetCosts(costs);
        selectionMenu.constructableName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";
    }

    public void EnableButtons()
    {
        UIManager.Instance.SetAllActionButtonsInactive();

        SelectionManager.Instance.CheckForAllowableSelection();
        SelectionManager.Instance.CheckForCancelableAction();
    }
}