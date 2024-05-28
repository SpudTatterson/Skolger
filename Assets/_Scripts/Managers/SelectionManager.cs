using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; private set; }
    List<ISelectable> currentSelected = new List<ISelectable>();
    SelectionType selectionType;
    ISelectionStrategy selectionStrategy;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More then one SelectionManager");
            Destroy(this);
        }
    }
    void Update()
    {
        HandleDeselectionInput();

        HandleSelection();
    }

    #region Selection Logic

    void HandleSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50, LayerManager.instance.SelectableLayerMask) &&
        hit.transform.TryGetComponent(out ISelectable selectable) &&
        Input.GetKeyDown(KeyCode.Mouse0) && !currentSelected.Contains(selectable))
        {
            selectable.OnSelect();
            SetSelectionType(selectable.GetSelectionType());
            UIManager.instance.selectionPanel.SetActive(true);
            Debug.Log("selected " + currentSelected.Count);
        }
    }

    void HandleDeselectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectAll();
        }
    }

    #endregion

    #region Public methods

    public void SetSelectionType(SelectionType selectionType)
    {
        if (currentSelected.Count > 1)
        {
            SetSelectionStrategy(new MultipleSelectionStrategy());
        }
        else
        {
            var selectable = currentSelected[0];
            switch (selectable.GetSelectionType())
            {
                case SelectionType.Item:
                    SetSelectionStrategy(new ItemSelectionStrategy());
                    break;
                case SelectionType.Constructable:
                    SetSelectionStrategy(new ConstructableSelectionStrategy());
                    break;
                case SelectionType.Harvestable:
                    SetSelectionStrategy(new HarvestableSelectionStrategy());
                    break;
                case SelectionType.Colonist:
                    SetSelectionStrategy(new ColonistSelectionStrategy());
                    break;
            }
        }
        selectionStrategy.ApplySelection(currentSelected);
    }
    void SetSelectionStrategy(ISelectionStrategy selectionStrategy)
    {
        if (this.selectionStrategy != null)
            this.selectionStrategy.CleanUp();
        this.selectionStrategy = selectionStrategy;
    }
    public void AddToCurrentSelected(ISelectable selectable)
    {
        currentSelected.Add(selectable);
    }
    public void RemoveFromCurrentSelected(ISelectable selectable)
    {
        currentSelected.Remove(selectable);
    }

    #endregion

    #region Buttons

    public void CheckForCancelableAction()
    {
        if (currentSelected[0].HasActiveCancelableAction())
        {
            UIManager.instance.EnableCancelButton();
        }
    }

    #endregion

    #region Button Actions

    public void SetToHarvest()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            selectable.GetGameObject().GetComponent<IHarvestable>().AddToHarvestQueue();
        }
        UIManager.instance.EnableCancelButton();
    }

    public void Allow()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            selectable.GetGameObject().GetComponent<IAllowable>().OnAllow();
        }
        UIManager.instance.EnableAllowDisallowButton(true);
    }
    public void Disallow()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            selectable.GetGameObject().GetComponent<IAllowable>().OnDisallow();
        }
        UIManager.instance.EnableAllowDisallowButton(false);
    }

    public void TryToCancelActions()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            GameObject GO = selectable.GetGameObject();
            if (GO.TryGetComponent(out IHarvestable harvestable))
            {
                harvestable.RemoveFromHarvestQueue();
                UIManager.instance.EnableHarvestableButtons();
            }
            if (GO.TryGetComponent(out IConstructable constructable))
            {
                constructable.CancelConstruction();
                bool allowed = GO.GetComponent<IAllowable>().IsAllowed();
                UIManager.instance.EnableConstructableButtons(allowed);
            }
        }
    }

    #endregion

    #region Cleanup
    void DeselectAll()
    {
        List<ISelectable> selectedCopy = new List<ISelectable>(currentSelected);

        foreach (ISelectable selectable in selectedCopy)
        {
            selectable.OnDeselect();
        }
        if (this.selectionStrategy != null)
            this.selectionStrategy.CleanUp();
        currentSelected.Clear();
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.selectionPanel.SetActive(false);
    }

    #endregion
}
