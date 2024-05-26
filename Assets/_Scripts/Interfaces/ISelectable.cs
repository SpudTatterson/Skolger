using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionType
{
    Item,
    Constructable,
    Harvestable,
    Colonist
}
public interface ISelectable
{
    public SelectionType GetSelectionType();
    public GameObject GetGameObject();

    public void OnSelect();
    public void OnDeselect();

    
}
