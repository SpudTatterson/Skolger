using System.Collections.Generic;

public class ConstructableSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        IAllowable allowable = selectedItems[0].GetGameObject().GetComponent<IAllowable>();

        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.constructableSelection.gameObject.SetActive(true);
        EnableButtons();

        IConstructable constructable = selectedItems[0].GetGameObject().GetComponent<IConstructable>();
        List<ItemCost> costs = constructable.GetAllCosts();

        ConstructableSelectionMenu selectionMenu = UIManager.instance.constructableSelection;

        selectionMenu.SetCosts(costs);
        selectionMenu.constructableName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";
    }

    public void EnableButtons()
    {
        UIManager.instance.SetAllActionButtonsInactive();

        SelectionManager.instance.CheckForAllowableSelection();
        SelectionManager.instance.CheckForCancelableAction();
    }
}