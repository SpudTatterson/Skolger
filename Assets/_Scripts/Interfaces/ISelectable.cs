using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionType
{
    Item,
    Constructable,
    Building,
    Workbench,
    Harvestable,
    Colonist,
    Multiple
}
public interface ISelectable
{
    public SelectionType GetSelectionType();
    public ISelectionStrategy GetSelectionStrategy();
    public GameObject GetGameObject();
    public string GetMultipleSelectionString(out int amount);
    bool HasActiveCancelableAction();

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.instance;
        manager.AddToCurrentSelected(this);
    }

    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.instance;
        manager.RemoveFromCurrentSelected(this);
    }

}
