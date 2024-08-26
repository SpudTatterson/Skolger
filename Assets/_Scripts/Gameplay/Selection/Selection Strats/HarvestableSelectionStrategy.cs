using System.Collections.Generic;

public class HarvestableSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.harvestableSelection.gameObject.SetActive(true);
        EnableButtons();

        IHarvestable harvestable = selectedItems[0]as IHarvestable;
        List<ItemDrop> drops = harvestable.GetItemDrops();

        HarvestableSelectionMenu selectionMenu = UIManager.Instance.harvestableSelection;

        selectionMenu.SetDrops(drops);
        selectionMenu.harvestableName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";

        SelectionManager.Instance.CheckForCancelableAction();
    }

    public void EnableButtons()
    {
        UIManager.Instance.SetAllActionButtonsInactive();
        UIManager.Instance.harvestButton.SetActive(true);
    }
}