using System.Collections.Generic;
using System.Linq;


public class ColonistSelectionStrategy : ISelectionStrategy
{
    UIManager ui;
    public void ApplySelection(List<ISelectable> selectedItems)
    {
        ui = UIManager.Instance;
        
        ColonistData colonist = selectedItems[0] as ColonistData;

        ui.colonistSelection.UpdateMenu(colonist);
       
        ui.SetAllSelectionUIInactive();
        ui.colonistSelection.gameObject.SetActive(true);
        EnableButtons();
    }

    public void EnableButtons()
    {
        if (ui == null)
            ui = UIManager.Instance;
        ui.SetAllActionButtonsInactive();
        // draft button
    }
}