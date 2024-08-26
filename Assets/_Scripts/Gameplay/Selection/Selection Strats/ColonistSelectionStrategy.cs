using System.Collections.Generic;
using System.Linq;


public class ColonistSelectionStrategy : ISelectionStrategy
{
    UIManager ui;
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        ui = UIManager.Instance;
        ui.SetAllSelectionUIInactive();
        ui.colonistSelection.gameObject.SetActive(true);
        EnableButtons();

        ColonistData colonist = selectedItems[0] as ColonistData;

        ui.colonistSelection.UpdateMenu(colonist);
    }

    public void EnableButtons()
    {
        ui.SetAllActionButtonsInactive();
        // draft button
    }
}