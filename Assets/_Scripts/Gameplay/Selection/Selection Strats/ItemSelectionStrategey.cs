using System.Collections.Generic;

public class ItemSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        IAllowable allowable = selectedItems[0] as IAllowable;

        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.itemSelection.gameObject.SetActive(true);
        EnableButtons();

        ItemObject selectedItem = selectedItems[0]as ItemObject;
        ItemSelectionMenu selectionMenu = UIManager.instance.itemSelection;

        selectionMenu.itemName.text = $"Item Name: {selectedItem.itemData.name}";
        selectionMenu.amount.text = $"Amount: {selectedItem.amount}";
        selectionMenu.stackSize.text = $"Stack Size: {selectedItem.itemData.stackSize}";
    }

    public void EnableButtons()
    {
        UIManager.instance.SetAllActionButtonsInactive();

        SelectionManager.Instance.CheckForAllowableSelection();
    }
}