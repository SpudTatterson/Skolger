using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionType
{
    None,
    Item,
    Constructable,
    Building,
    Workbench,
    Harvestable,
    Colonist,
    Stockpile,
    Multiple
}
public interface ISelectable
{
    public SelectionType GetSelectionType();
    public ISelectionStrategy GetSelectionStrategy();
    public string GetMultipleSelectionString(out int amount);
    bool HasActiveCancelableAction();

    public bool IsSelected { get; }

    public void OnSelect();
    public void OnDeselect();

    public void OnHover();
    public void OnHoverEnd();

}
