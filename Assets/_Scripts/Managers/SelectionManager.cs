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
    [SerializeField] List<SelectionType> specialSelectionTypes;
    [SerializeField] float dragDelay = 0.1f;

    Vector3 mouseStartPos;
    Vector3 mouseEndPos;
    float mouseDownTime;
    SelectionAction selectionAction;
    enum SelectionAction
    {
        Default,
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
        if (!isSelecting) return;

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
            hit.transform.TryGetComponent(out ISelectable selectable))
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
            ExtendedDebug.DrawBox(center, halfExtents * 2, Quaternion.identity);


            //on hover
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && mouseStartPos != Vector3.zero)// stopped dragging
        {
            mouseEndPos = Input.mousePosition;
            UIManager.instance.selectionBoxImage.gameObject.SetActive(false);
            Vector3 halfExtents = VectorUtility.ScreenBoxToWorldBoxGridAligned(mouseStartPos, mouseEndPos, 1, LayerManager.instance.GroundLayerMask,
             out Vector3 center);
            List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(center, halfExtents * 0.95f);

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
            selectionAction = SelectionAction.Default;

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
        if (selectables.Count > 0)
        {
            UIManager.instance.selectionPanel.SetActive(true);
        }
        else
        {
            DeselectAll();
            return;
        }

        if (selectionAction == SelectionAction.Add)
        {
            foreach (var selectable in selectables)
            {
                if (!currentSelected.Contains(selectable))
                {
                    selectable.OnSelect();
                }
            }
            SetSelectionType(selectables[0].GetSelectionType());
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
                SetSelectionType(currentSelected[0].GetSelectionType());
            }
            else
                DeselectAll();
            return;
        }
        else if (selectionAction == SelectionAction.Default)
        {
            if (selectables.Count > 0 && currentSelected.Count > 0 && selectables[0] == currentSelected[0])
            {
                TryToSelectOtherItemInCell();
            }
            else
            {
                currentSelected.Clear();

                foreach (var selectable in selectables)
                {
                    bool doBreak = false;
                    if (CheckForSpecialSelectionCase(selectable, out SelectionType type))
                    {
                        if (type == SelectionType.Stockpile)
                        {
                            if (selectable == selectables[0] && selectables.Count == 1)
                                doBreak = true;
                            else
                                continue;

                        }
                    }

                    selectable.OnSelect();

                    if (doBreak) break;
                }
                SetSelectionType(selectables[0].GetSelectionType());
            }
        }



    }

    bool CheckForSpecialSelectionCase(ISelectable selectable, out SelectionType selectionType)
    {
        foreach (SelectionType type in specialSelectionTypes)
        {
            if (selectable.GetSelectionType() == type)
            {
                selectionType = type;
                return true;
            }
        }
        selectionType = default;
        return false;
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
            SetSelectionStrategy(selectable.GetSelectionStrategy());
        }
        selectionStrategy.ApplySelection(currentSelected);
    }
    void SetSelectionStrategy(ISelectionStrategy selectionStrategy)
    {
        this.selectionStrategy?.CleanUp();
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
    public void CheckForAllowableSelection()
    {
        if (currentSelected[0] is IAllowable)
        {
            UIManager.instance.EnableAllowDisallowButton((currentSelected[0] as IAllowable).IsAllowed());
        }
    }

    #endregion

    #region Button Actions

    public void SetToHarvest()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            (selectable as IHarvestable).AddToHarvestQueue();
        }
        UIManager.instance.EnableCancelButton();
    }

    public void Allow()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            (selectable as IAllowable).OnAllow();
        }
        UIManager.instance.EnableAllowDisallowButton(true);
    }
    public void Disallow()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            (selectable as IAllowable).OnDisallow();
        }
        UIManager.instance.EnableAllowDisallowButton(false);
    }

    public void TryToCancelActions()
    {
        foreach (ISelectable selectable in currentSelected)
        {
            if (selectable is IHarvestable)
            {
                (selectable as IHarvestable).RemoveFromHarvestQueue();
                selectable.GetSelectionStrategy().EnableButtons();
            }
            if (selectable is IConstructable)
            {
                (selectable as IConstructable).CancelConstruction();
                selectable.GetSelectionStrategy().EnableButtons();
            }
        }
    }
    public void TryToDeconstruct()
    {
        foreach (ISelectable selectable in currentSelected)
        {

            if (selectable is BuildingObject)
            {
                (selectable as BuildingObject).Deconstruct();
            }
            if (selectable is Stockpile)
            {
                (selectable as Stockpile).DestroyStockpile();
            }
        }
    }

    public void TryToGrowZone()
    {
        if (currentSelected[0] is Stockpile)
        {
            Stockpile stockpile = currentSelected[0] as Stockpile;
            StockpilePlacer.instance.GrowStockpile(stockpile);
        }
    }
    public void TryToShrinkZone()
    {
        if (currentSelected[0] is Stockpile)
        {
            Stockpile stockpile = currentSelected[0] as Stockpile;
            StockpilePlacer.instance.ShrinkStockpile(stockpile);
        }
    }
    public void TryToSelectOtherItemInCell()
    {
        ISelectable selectable = currentSelected[0];
        Cell cell = GridManager.instance.GetCellFromPosition((selectable as MonoBehaviour).transform.position);
        Vector3 corner1 = cell.position - new Vector3(1f / 2, 0, 1f / 2);
        Vector3 corner2 = cell.position + new Vector3(1f / 2, 0, 1f / 2);
        Vector3 halfSize = new Vector3(Mathf.Abs(corner1.x - corner2.x), 3f, Mathf.Abs(corner1.z - corner2.z) / 2f);

        List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(cell.position, halfSize * 0.95f);
        selectables.Remove(selectable);

        if (selectables.Count != 0)
        {
            currentSelected.Clear();
            selectables[0].OnSelect();
            SetSelectionType(selectables[0].GetSelectionType());
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
