using System.Collections.Generic;

public class ConstructableSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        IAllowable allowable = selectedItems[0].GetGameObject().GetComponent<IAllowable>();

        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.constructableSelection.gameObject.SetActive(true);
        UIManager.instance.EnableConstructableButtons(allowable.IsAllowed());

        IConstructable constructable = selectedItems[0].GetGameObject().GetComponent<IConstructable>();
        List<ItemCost> costs = constructable.GetAllCosts();

        ConstructableSelectionMenu selectionMenu = UIManager.instance.constructableSelection;

        selectionMenu.SetCosts(costs);
        selectionMenu.constructableName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";
    }
}