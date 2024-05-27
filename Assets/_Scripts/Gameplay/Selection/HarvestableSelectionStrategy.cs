using System.Collections.Generic;

public class HarvestableSelectionStrategy : ISelectionStrategy
{
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.harvestableSelection.gameObject.SetActive(true);
        UIManager.instance.EnableHarvestableButtons();

        IHarvestable harvestable = selectedItems[0].GetGameObject().GetComponent<IHarvestable>();
        List<ItemDrop> drops = harvestable.GetItemDrops();

        HarvestableSelectionMenu selectionMenu = UIManager.instance.harvestableSelection;

        selectionMenu.SetDrops(drops);
        selectionMenu.harvestableName.text = $"Name: {selectedItems[0].GetMultipleSelectionString(out _)}";

        SelectionManager.instance.CheckForCancelableAction();
    }
}