using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public bool isSelecting = true;
    public static SelectionManager instance { get; private set; }
    List<ISelectable> currentSelected = new List<ISelectable>();
    SelectionType selectionType;
    ISelectionStrategy selectionStrategy;
    [SerializeField] float dragDelay = 0.1f;

    Vector3 mouseStartPos;
    Vector3 mouseEndPos;
    float mouseDownTime;
    SelectionAction selectionAction;
    enum SelectionAction
    {
        Single,
        Add,
        Remove
    }
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
        if(!isSelecting) return;
        
        HandleSelectionAction();

        HandleDeselectionInput();

        HandleSelectionInput();
    }

    #region Selection Logic

    void HandleSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())// started clicking
        {
            mouseStartPos = Input.mousePosition;
            mouseDownTime = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && mouseDownTime + dragDelay > Time.time)//not dragging player only clicked
        {
            List<ISelectable> selectables = new List<ISelectable>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.SphereCast(ray, 1, out RaycastHit hit, 50, LayerManager.instance.SelectableLayerMask) &&
            hit.transform.TryGetComponent(out ISelectable selectable) && !currentSelected.Contains(selectable))
            {
                selectables.Add(selectable);
            }
            mouseStartPos = Vector3.zero;
            mouseEndPos = Vector3.zero;
            mouseDownTime = 0;
            Select(selectables);
        }
        else if (mouseDownTime + dragDelay > Time.time)
            return;
        else if (Input.GetKey(KeyCode.Mouse0) && mouseStartPos != Vector3.zero)// started dragging
        {
            mouseEndPos = Input.mousePosition;
            //UIManager.instance.ResizeSelectionBox(mouseStartPos, mouseEndPos);
            Vector3 halfExtents = VectorUtility.ScreenBoxToWorldBoxGridAligned(mouseStartPos, mouseEndPos, 1, LayerManager.instance.GroundLayerMask, out Vector3 center);
            List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(center, halfExtents);
            ExtendedDebug.DrawBox(center, halfExtents * 2, Quaternion.identity);


            //on hover
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && mouseStartPos != Vector3.zero)// stopped dragging
        {
            mouseEndPos = Input.mousePosition;
            UIManager.instance.selectionBoxImage.gameObject.SetActive(false);
            Vector3 halfExtents = VectorUtility.ScreenBoxToWorldBoxGridAligned(mouseStartPos, mouseEndPos, 1, LayerManager.instance.GroundLayerMask,
             out Vector3 center);
            List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(center, halfExtents);

            Select(selectables);

            mouseStartPos = Vector3.zero;
            mouseEndPos = Vector3.zero;
            mouseDownTime = 0;
        }
    }

    void HandleSelectionAction()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            selectionAction = SelectionAction.Add;
            return;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            selectionAction = SelectionAction.Remove;
            return;
        }
        else
            selectionAction = SelectionAction.Single;

    }

    void HandleDeselectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectAll();
        }
    }

    void Select(List<ISelectable> selectables)
    {
        if (selectionAction == SelectionAction.Add)
        {
            foreach (var selectable in selectables)
            {
                if (!currentSelected.Contains(selectable))
                {
                    selectable.OnSelect();
                    SetSelectionType(selectable.GetSelectionType());
                }
            }
        }
        else if (selectionAction == SelectionAction.Remove)
        {
            foreach (var selectable in selectables)
            {
                if (currentSelected.Contains(selectable))
                {
                    selectable.OnDeselect();
                }
            }
            if (currentSelected.Count != 0)
            {
                foreach (var selectable in currentSelected)
                {
                    SetSelectionType(selectable.GetSelectionType());
                }
            }
            else
                DeselectAll();
            return;
        }
        else if (selectionAction == SelectionAction.Single)
        {
            currentSelected.Clear();
            foreach (var selectable in selectables)
            {
                if (!currentSelected.Contains(selectable))
                {
                    selectable.OnSelect();
                    SetSelectionType(selectable.GetSelectionType());
                }
            }
        }

        if (selectables.Count > 0)
        {
            UIManager.instance.selectionPanel.SetActive(true);
        }
        else
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
                case SelectionType.Building:
                    SetSelectionStrategy(new BuildingSelectionStrategy());
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
        selectionStrategy?.CleanUp();

        currentSelected.Clear();
        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.selectionPanel.SetActive(false);
    }

    #endregion
}
