using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoSingleton<SelectionManager>
{
    public bool isSelecting = true;
    List<ISelectable> currentSelected = new List<ISelectable>();
    ISelectionStrategy selectionStrategy;
    [SerializeField] List<SelectionType> specialSelectionTypes;

    [SerializeField] float dragDelay = 0.1f;
    GameObject tempSelectionGrid;
    Vector3 worldMouseStartPos;
    Vector3 worldMouseEndPos;
    float mouseDownTime;
    Cell lastCell;
    Cell firstCell;
    [SerializeField] SelectionAction selectionAction;

    List<ISelectable> LastHovered = new List<ISelectable>();

    bool setToUpdate;

    void Update()
    {
        if (!isSelecting) return;

        HandleSelectionAction();

        HandleDeselectionInput();

        HandleSelectionInput();

        if (setToUpdate)
        {
            if (currentSelected.Count != 0)
            {
                SetSelectionType(currentSelected[0].GetSelectionType());
                UIManager.Instance.selectionPanel.SetActive(true);
            }
            else
                ResetSelection();

            setToUpdate = false;
        }
    }

    #region Selection Logic

    void HandleSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartSelection();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            EndSelection();
        }
        else if (Input.GetKey(KeyCode.Mouse0) && worldMouseStartPos != Vector3.zero)
        {
            DragSelection();
            VisualizeSelection();
        }
    }

    void StartSelection()
    {
        worldMouseStartPos = VectorUtility.ScreeToWorldPosition(Input.mousePosition, LayerManager.Instance.GroundLayerMask);
        mouseDownTime = Time.time;
    }

    void EndSelection()
    {
        ResetHovered();
        if (mouseDownTime + dragDelay > Time.time)
        {
            ClickSelection();
        }
        else if (worldMouseStartPos != Vector3.zero)
        {
            BoxSelection();
        }
        ResetDrag();
    }

    void DragSelection()
    {
        worldMouseEndPos = VectorUtility.ScreeToWorldPosition(Input.mousePosition, LayerManager.Instance.GroundLayerMask);
    }

    void VisualizeSelection()
    {
        firstCell = GridManager.Instance.GetCellFromPosition(worldMouseStartPos);
        Cell lastCell = GridManager.Instance.GetCellFromPosition(worldMouseEndPos);

        if (this.lastCell != lastCell && lastCell != null)
        {
            DrawSelection(firstCell, lastCell);

            GetHovered(firstCell, lastCell);
            this.lastCell = lastCell;
        }
    }

    void GetHovered(Cell firstCell, Cell lastCell)
    {
        Box box = VectorUtility.CalculateBoxSize(firstCell.position, lastCell.position);

        box.ShrinkBoxNoY(0.95f);

        ExtendedDebug.DrawBox(box.center, box.halfExtents * 2, Quaternion.identity); // visualize in editor

        //Trigger OnHover
        List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(box.center, box.halfExtents);
        ResetHovered();
        foreach (ISelectable selectable in selectables)
        {
            selectable.OnHover();
            LastHovered.Add(selectable);
        }
    }

    void DrawSelection(Cell firstCell, Cell lastCell)
    {
        Destroy(tempSelectionGrid);
        //Draw Selection in the world
        List<Cell> cells = new SquarePlacementStrategy().GetCells(firstCell, lastCell);
        tempSelectionGrid = MeshUtility.CreateGridMesh(cells, "SelectionGrid", MaterialManager.Instance.SelectionMaterial);
    }

    void ClickSelection()
    {
        List<ISelectable> selectables = new List<ISelectable>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.SphereCast(ray, 1, out RaycastHit hit, 50, LayerManager.Instance.SelectableLayerMask) &&
        hit.transform.TryGetComponent(out ISelectable selectable))
        {
            selectables.Add(selectable);
        }
        Select(selectables);
    }

    void BoxSelection()
    {
        Box box = VectorUtility.CalculateBoxSizeGridAligned(firstCell, lastCell, 1f);
        box.ShrinkBoxNoY(0.95f);
        List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(box.center, box.halfExtents);
        Select(selectables);
    }

    void ResetMouseData()
    {
        worldMouseStartPos = Vector3.zero;
        worldMouseEndPos = Vector3.zero;
        mouseDownTime = 0;
    }

    void HandleSelectionAction()
    {
        if (selectionAction == SelectionAction.Default || selectionAction == SelectionAction.Add || selectionAction == SelectionAction.Remove)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                selectionAction = SelectionAction.Add;
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                selectionAction = SelectionAction.Remove;
            else
                selectionAction = SelectionAction.Default;
        }
    }

    void HandleDeselectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            ResetSelection();
        }
    }

    void Select(List<ISelectable> selectables)
    {
        if (selectables.Count == 0)
        {
            ResetSelection();
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
            RemoveSelection(selectables);
        }
        else if (selectionAction == SelectionAction.Default)
        {
            DefaultSelection(selectables);
        }
        else if (selectionAction == SelectionAction.Harvest)
        {
            SetSelectionForHarvest(selectables);
        }
        else if (selectionAction == SelectionAction.Allow)
        {
            SetSelectionAllowed(selectables);
        }
        else if (selectionAction == SelectionAction.Forbid)
        {
            SetSelectionForbidden(selectables);
        }
        else if (selectionAction == SelectionAction.Deconstruct)
        {
            SetSelectionForDestruction(selectables);
        }
        else if (selectionAction == SelectionAction.Cancel)
        {
            CancelSelection(selectables);
        }


    }

    void DefaultSelection(List<ISelectable> selectables)
    {
        if (selectables.Count == 1 && currentSelected.Count > 0 && selectables[0] == currentSelected[0])
        {
            TryToSelectOtherItemInCell();
        }
        else
        {
            DeselectAll();

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
                    if (type == SelectionType.Colonist)
                    {
                        DeselectAll();
                        doBreak = true;
                    }
                }

                selectable.OnSelect();

                if (doBreak) break;
            }
            UpdateSelection();
        }
    }

    void RemoveSelection(List<ISelectable> selectables)
    {
        foreach (var selectable in selectables)
        {
            selectable.OnDeselect();
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

    public void SetNewSelectionAction(SelectionAction selectionAction)
    {
        this.selectionAction = selectionAction;
        if(selectionAction != SelectionAction.Default && selectionAction != SelectionAction.Add && selectionAction != SelectionAction.Remove)
        {
            UIManager.Instance.SelectionActionCanvas.SetActive(true);
            UIManager.Instance.actionText.text = $"Left Click + Drag to {selectionAction.ToString()}";
        }
    }
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
    public void UpdateSelection()
    {
        setToUpdate = true;
    }

    #endregion

    #region Buttons Logic

    public void CheckForCancelableAction()
    {
        if (currentSelected[0].HasActiveCancelableAction())
        {
            UIManager.Instance.EnableCancelButton();
        }
    }
    public void CheckForAllowableSelection()
    {
        if (currentSelected[0] is IAllowable)
        {
            UIManager.Instance.EnableAllowDisallowButton((currentSelected[0] as IAllowable).IsAllowed());
        }
    }

    #endregion

    #region Button Actions

    public void SetToHarvest()
    {
        SetSelectionForHarvest(currentSelected);
        UIManager.Instance.EnableCancelButton();
    }

    void SetSelectionForHarvest(List<ISelectable> selectables)
    {
        foreach (ISelectable selectable in selectables)
        {
            if (selectable is IHarvestable)
                (selectable as IHarvestable).AddToHarvestQueue();
        }
    }

    public void Allow()
    {
        SetSelectionAllowed(currentSelected);
        UIManager.Instance.EnableAllowDisallowButton(true);
    }

    void SetSelectionAllowed(List<ISelectable> selectables)
    {
        foreach (ISelectable selectable in selectables)
        {
            if (selectable is IAllowable)
                (selectable as IAllowable).OnAllow();
        }
    }

    public void Forbid()
    {
        SetSelectionForbidden(currentSelected);
        UIManager.Instance.EnableAllowDisallowButton(false);
    }

    void SetSelectionForbidden(List<ISelectable> selectables)
    {
        foreach (ISelectable selectable in selectables)
        {
            if (selectable is IAllowable)
                (selectable as IAllowable).OnDisallow();
        }
    }

    public void TryToCancelActions()
    {
        CancelSelection(currentSelected);
    }

    void CancelSelection(List<ISelectable> selectables)
    {
        for (int i = 0; i < selectables.Count; i++)
        {
            ISelectable selectable = selectables[i];
            if (selectable is IHarvestable)
            {
                (selectable as IHarvestable).RemoveFromHarvestQueue();
            }
            if (selectable is IConstructable)
            {
                (selectable as IConstructable).CancelConstruction();
            }
        }
    }

    public void TryToDeconstruct()
    {
        SetSelectionForDestruction(currentSelected);
    }

    void SetSelectionForDestruction(List<ISelectable> selectables)
    {
        for (int i = 0; i < selectables.Count; i++)
        {
            ISelectable selectable = selectables[i];
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
            StockpilePlacer.Instance.GrowStockpile(stockpile);
        }
    }
    public void TryToShrinkZone()
    {
        if (currentSelected[0] is Stockpile)
        {
            Stockpile stockpile = currentSelected[0] as Stockpile;
            StockpilePlacer.Instance.ShrinkStockpile(stockpile);
        }
    }
    public void TryToSelectOtherItemInCell()
    {
        ISelectable selectable = currentSelected[0];
        Cell cell = GridManager.Instance.GetCellFromPosition((selectable as MonoBehaviour).transform.position);
        Vector3 corner1 = cell.position - new Vector3(1f / 2, 0, 1f / 2);
        Vector3 corner2 = cell.position + new Vector3(1f / 2, 0, 1f / 2);
        Box box = VectorUtility.CalculateBoxSize(corner1, corner2).ShrinkBoxNoY(0.95f);

        List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(cell.position, box.halfExtents);
        selectables.Remove(selectable);

        if (selectables.Count != 0)
        {
            DeselectAll();
            selectables[0].OnSelect();
            SetSelectionType(selectables[0].GetSelectionType());
        }
    }

    public void FocusCameraToSelected()
    {
        List<Vector3> selectedPositions = new List<Vector3>();
        foreach (ISelectable selectable in currentSelected)
        {
            selectedPositions.Add((selectable as MonoBehaviour).transform.position);
        }

        Vector3 center = VectorUtility.CalculateCenter(selectedPositions);
        StartCoroutine(CameraController.Instance.SendCameraToTarget(center));
    }

    #endregion

    #region Cleanup
    void ResetSelection()
    {
        selectionAction = SelectionAction.Default;
        DeselectAll();
        ResetDrag();
        ResetHovered();

        UIManager.Instance.SetAllSelectionUIInactive();
        UIManager.Instance.selectionPanel.SetActive(false);
        UIManager.Instance.SelectionActionCanvas.SetActive(false);
    }

    void DeselectAll()
    {
        List<ISelectable> selectedCopy = new List<ISelectable>(currentSelected);

        foreach (ISelectable selectable in selectedCopy)
        {
            selectable.OnDeselect();
        }
        currentSelected.Clear();

        selectionStrategy?.CleanUp();
    }

    void ResetHovered()
    {
        for (int i = 0; i < LastHovered.Count; i++)
        {
            LastHovered[i].OnHoverEnd();
        }
        LastHovered.Clear();
    }

    void ResetDrag()
    {
        ResetMouseData();
        Destroy(tempSelectionGrid);
    }

    #endregion
}
