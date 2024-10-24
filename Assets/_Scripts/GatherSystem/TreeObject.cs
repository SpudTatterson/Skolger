using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseHarvestable : MonoBehaviour, IHarvestable, ISelectable, ICellOccupier
{
    [SerializeField] public ParticleSystem hitEffect;
    [SerializeField] private AudioClip[] ChoppingSound;
    [SerializeField, Tooltip("Amount of time to wait between playing effects")] float effectsWaitTime = 1f;

    [SerializeField] float baseGatherTime = 5f;

    [SerializeField] List<ItemDrop> drops = new List<ItemDrop>();
    [SerializeField] string harvestableName = "";
    BillBoard setForHarvestBillboard;
    Outline outline;
    FillBar fillBar;
    float timeHarvesting = 0f;
    public Cell cornerCell { get; private set; }
    bool beingHarvested = false;
    bool finishedHarvesting = false;
    bool setForHarvesting = false;
    public event System.Action OnHarvested;

    public bool IsSelected { get; private set; }

    void Awake()
    {
        setForHarvestBillboard = GetComponentInChildren<BillBoard>(true);
        outline = GetComponentInChildren<Outline>(true);
        fillBar = GetComponentInChildren<FillBar>(true);
    }
    public void Harvest()
    {
        OnHarvested?.Invoke();
        foreach (ItemDrop drop in drops)
        {
            int amount = Random.Range(drop.minDropAmount, drop.maxDropAmount);
            if (amount == 0) continue;
            Cell dropCell = cornerCell.GetClosestEmptyCell();
            ItemObject.MakeInstance(drop.itemData, amount, dropCell.position);
        }
        TaskManager.Instance.RemoveFromHarvestQueue(this);
        finishedHarvesting = true;
    }

    public IEnumerator StartHarvesting()
    {
        timeHarvesting = 0f;
        beingHarvested = true;
        fillBar.UpdateMaxFillAmount(baseGatherTime); // Multiply by any modifiers

        float timeSincePlayedEffect = 0;

        while (timeHarvesting < baseGatherTime)
        {
            timeHarvesting += Time.deltaTime;
            timeSincePlayedEffect += Time.deltaTime;

            if (timeSincePlayedEffect >= effectsWaitTime)
            {
                HitTree();
                fillBar.UpdateFillAmount(timeHarvesting);
                timeSincePlayedEffect = 0f;
            }

            
            yield return null;
        }

        beingHarvested = false;
        Harvest();
    }


    void HitTree()
    {
        SoundsFXManager.Instance.PlayRandomSoundFXClip(ChoppingSound, transform, 1f);
        transform.DOShakeRotation(0.15f, 5);

        hitEffect.Play();
        
    }

    public bool IsBeingHarvested()
    {
        return beingHarvested;
    }
    public bool FinishedHarvesting()
    {
        return finishedHarvesting;
    }
    public void AddToHarvestQueue()
    {
        if (!setForHarvesting)
        {
            TaskManager.Instance.AddToHarvestQueue(this);
        }
        ShowBillboard();
        setForHarvesting = true;
    }

    public void RemoveFromHarvestQueue()
    {
        TaskManager.Instance.RemoveFromHarvestQueue(this);
        DisableBillboard();
        beingHarvested = false;
        setForHarvesting = false;
    }
    public List<ItemDrop> GetItemDrops()
    {
        return drops;
    }

    void ShowBillboard()
    {
        setForHarvestBillboard?.gameObject.SetActive(true);
    }
    void DisableBillboard()
    {
        setForHarvestBillboard?.gameObject.SetActive(false);
    }
    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.AddToCurrentSelected(this);
        IsSelected = true;

        outline?.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.RemoveFromCurrentSelected(this);
        if (IsSelected)
            manager.UpdateSelection();

        outline?.Disable();
        IsSelected = false;
    }
    public void OnHover()
    {
        outline?.Enable();
    }

    public void OnHoverEnd()
    {
        if (!IsSelected)
            outline?.Disable();
    }
    public SelectionType GetSelectionType()
    {
        return SelectionType.Harvestable;
    }
    public ISelectionStrategy GetSelectionStrategy()
    {
        return new HarvestableSelectionStrategy();
    }


    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return harvestableName;
    }


    public bool HasActiveCancelableAction()
    {
        return setForHarvesting;
    }

    #region ICellOccupier

    public void GetOccupiedCells()
    {
        cornerCell = GridManager.Instance.GetCellFromPosition(transform.position);
    }
    public void OnOccupy()
    {
        if (cornerCell == null) GetOccupiedCells();
        cornerCell.inUse = true;
        cornerCell.walkable = false;
    }

    public void OnRelease()
    {
        cornerCell.inUse = false;
        cornerCell.walkable = true;
    }

    #endregion

    void OnEnable()
    {
        OnOccupy();
    }
    void OnDisable()
    {
        OnRelease();
        OnDeselect();
    }
}
