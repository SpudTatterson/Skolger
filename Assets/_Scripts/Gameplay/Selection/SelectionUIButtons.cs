using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUIButtons : MonoBehaviour
{
    SelectionManager selectionManager;
    void Start()
    {
        selectionManager = SelectionManager.Instance;
    }
    public void SetActionToDefault()
    {
        selectionManager.SetNewSelectionAction(SelectionAction.Default);
    }
    public void SetActionToHarvest()
    {
        selectionManager.SetNewSelectionAction(SelectionAction.Harvest);
    }
    public void SetActionToAllow()
    {
        selectionManager.SetNewSelectionAction(SelectionAction.Allow);
    }
    public void SetActionToForbid()
    {
        selectionManager.SetNewSelectionAction(SelectionAction.Forbid);
    }
    public void SetActionToDeconstruct()
    {
        selectionManager.SetNewSelectionAction(SelectionAction.Deconstruct);
    }
    public void SetActionToCancel()
    {
        selectionManager.SetNewSelectionAction(SelectionAction.Cancel);
    }
}

public enum SelectionAction
{
    Default,
    Add,
    Remove,
    Harvest,
    Forbid,
    Allow,
    Deconstruct,
    Cancel,
}