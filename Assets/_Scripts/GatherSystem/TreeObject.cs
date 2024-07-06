using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : MonoBehaviour, IHarvestable, ISelectable, ICellOccupier
{
    [SerializeField] float baseGatherTime = 5f; 

    [SerializeField] List<ItemDrop> drops = new List<ItemDrop>();
    float timeHarvesting = 0f;
    public Cell cornerCell { get; private set; }
    bool beingHarvested = false;
    bool setForHarvesting = false;
    Harvester harvester;
    SelectionType selectionType = SelectionType.Harvestable;
    public void Harvest()
    {
        foreach (ItemDrop drop in drops)
        {
            int amount = Random.Range(drop.minDropAmount, drop.maxDropAmount);
            if (amount == 0) continue;
            Cell dropCell = cornerCell.GetClosestEmptyCell();
            ItemObject.MakeInstance(drop.itemData, amount, dropCell.position);
        }
        harvester.RemoveFromHarvestQueue(this);
        Destroy(this.gameObject);
    }

    public IEnumerator StartHarvesting()
    {
        timeHarvesting = 0f;
        beingHarvested = true;
        Debug.Log("started harvesting");

        while (timeHarvesting < baseGatherTime)
        {
            timeHarvesting += Time.deltaTime;
            yield return null;
        }

        beingHarvested = false;
        Harvest();
    }
    public bool IsBeingHarvested()
    {
        return beingHarvested;
    }
    public void AddToHarvestQueue()
    {
        if (!setForHarvesting)
        {
            harvester = FindObjectOfType<Harvester>();
            harvester.AddToHarvestQueue(this);
        }
        setForHarvesting = true;
    }
    public void RemoveFromHarvestQueue()
    {
        harvester.RemoveFromHarvestQueue(this);
        beingHarvested = false;
        setForHarvesting = false;
    }
    public List<ItemDrop> GetItemDrops()
    {
        return drops;
    }

    public SelectionType GetSelectionType()
    {
        return selectionType;
    }
    public ISelectionStrategy GetSelectionStrategy()
    {
        return new HarvestableSelectionStrategy();
    }


    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return "Tree";
    }


    public bool HasActiveCancelableAction()
    {
        return setForHarvesting;
    }

    #region ICellOccupier

    public void GetOccupiedCells()
    {
        cornerCell = FindObjectOfType<GridManager>().GetCellFromPosition(transform.position);
    }
    public void OnOccupy()
    {
        if (cornerCell == null) cornerCell = GridManager.instance.GetCellFromPosition(transform.position);
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
    }
}
