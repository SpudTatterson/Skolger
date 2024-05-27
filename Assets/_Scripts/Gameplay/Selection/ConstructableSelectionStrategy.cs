using System.Collections.Generic;

public class ConstructableSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.constructableSelection.gameObject.SetActive(true);
        UIManager.instance.EnableConstructableButtons();

        IConstructable constructable = selectedItems[0].GetGameObject().GetComponent<IConstructable>();
        List<ItemCost> costs = constructable.GetAllCosts();

        ConstructableSelectionMenu selectionMenu = UIManager.instance.constructableSelection;

        selectionMenu.SetCosts(costs);
        selectionMenu.constructableName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";

        SelectionManager.instance.CheckForCancelableAction();
    }
}