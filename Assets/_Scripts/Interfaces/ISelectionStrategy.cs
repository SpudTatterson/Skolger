using System.Collections.Generic;

public interface ISelectionStrategy
{
    void ApplySelection(List<ISelectable> selectedItems);

    void CleanUp()
    {
        
    }
    void EnableButtons();
}