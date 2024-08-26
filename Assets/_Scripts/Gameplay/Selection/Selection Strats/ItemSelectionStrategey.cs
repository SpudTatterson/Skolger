using System.Collections.Generic;

public class ItemSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        IAllowable allowable = selectedItems[0] as IAllowable;

        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.itemSelection.gameObject.SetActive(true);
        EnableButtons();

        ItemObject selectedItem = selectedItems[0]as ItemObject;
        ItemSelectionMenu selectionMenu = UIManager.Instance.itemSelection;

        selectionMenu.itemName.text = $"Item Name: {selectedItem.itemData.name}";
        selectionMenu.amount.text = $"Amount: {selectedItem.amount}";
        selectionMenu.stackSize.text = $"Stack Size: {selectedItem.itemData.stackSize}";
    }

    public void EnableButtons()
    {
        UIManager.Instance.SetAllActionButtonsInactive();

        SelectionManager.Instance.CheckForAllowableSelection();
    }
}