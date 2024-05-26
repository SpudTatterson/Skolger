using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionType
{
    Item,
    Constructable,
    Harvestable,
    Colonist,
    Multiple
}
public interface ISelectable
{
    public SelectionType GetSelectionType();
    public GameObject GetGameObject();
    public string GetMultipleSelectionString(out int amount);

    public void OnSelect();
    public void OnDeselect();


}
