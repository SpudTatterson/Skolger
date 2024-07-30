using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public bool isSelecting = true;
    public static SelectionManager instance { get; private set; }
    List<ISelectable> currentSelected = new List<ISelectable>();
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

    bool setToUpdate;
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

        if (setToUpdate)
        {
            if (currentSelected.Count != 0)
            {
                SetSelectionType(currentSelected[0].GetSelectionType());
                UIManager.instance.selectionPanel.SetActive(true);
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
        else if (Input.GetKey(KeyCode.Mouse0) && mouseStartPos != Vector3.zero)
        {
            DragSelection();
        }
    }

    void StartSelection()
    {
        mouseStartPos = Input.mousePosition;
        mouseDownTime = Time.time;
    }

    void EndSelection()
    {
        if (mouseDownTime + dragDelay > Time.time)
        {
            ClickSelection();
        }
        else if (mouseStartPos != Vector3.zero)
        {
            BoxSelection();
        }
        ResetMouseData();
    }

    void DragSelection()
    {
        mouseEndPos = Input.mousePosition;
        Vector3 halfExtents = VectorUtility.ScreenBoxToWorldBoxGridAligned(mouseStartPos, mouseEndPos, 1, LayerManager.instance.GroundLayerMask, out Vector3 center);
        ExtendedDebug.DrawBox(center, halfExtents * 2, Quaternion.identity);
    }

    void ClickSelection()
    {
        List<ISelectable> selectables = new List<ISelectable>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.SphereCast(ray, 1, out RaycastHit hit, 50, LayerManager.instance.SelectableLayerMask) &&
        hit.transform.TryGetComponent(out ISelectable selectable))
        {
            selectables.Add(selectable);
        }
        Select(selectables);
    }

    void BoxSelection()
    {
        Vector3 halfExtents = VectorUtility.ScreenBoxToWorldBoxGridAligned(mouseStartPos, mouseEndPos, 1, LayerManager.instance.GroundLayerMask, out Vector3 center);
        List<ISelectable> selectables = ComponentUtility.GetComponentsInBox<ISelectable>(center, halfExtents * 0.95f);
        Select(selectables);
    }

    void ResetMouseData()
    {
        mouseStartPos = Vector3.zero;
        mouseEndPos = Vector3.zero;
        mouseDownTime = 0;
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
            foreach (var selectable in selectables)
            {
                selectable.OnDeselect();
            }
            return;
        }
        else if (selectionAction == SelectionAction.Default)
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
                    }

                    selectable.OnSelect();

                    if (doBreak) break;
                }
                UpdateSelection();
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
    public void UpdateSelection()
    {
        setToUpdate = true;
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
        for (int i = 0; i < currentSelected.Count; i++)
        {
            ISelectable selectable = currentSelected[i];
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
    void ResetSelection()
    {
        DeselectAll();

        UIManager.instance.SetAllSelectionUIInactive();
        UIManager.instance.selectionPanel.SetActive(false);
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

    #endregion
}
