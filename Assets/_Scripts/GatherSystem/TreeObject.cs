using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : MonoBehaviour, IHarvestable, ISelectable
{
    [SerializeField] float baseGatherTime = 5f;

    [SerializeField] List<ItemDrop> drops = new List<ItemDrop>();
    float timeHarvesting = 0f;
    Cell occupiedCell;
    bool beingHarvested = false;
    bool setForHarvesting = false;
    Harvester harvester;
    SelectionType selectionType = SelectionType.Harvestable;
    public void Harvest()
    {
        occupiedCell.inUse = false;
        occupiedCell.Walkable = true;
        foreach (ItemDrop drop in drops)
        {
            int amount = Random.Range(drop.minDropAmount, drop.maxDropAmount);
            if (amount == 0) continue;
            Cell dropCell = occupiedCell.GetClosestEmptyCell();
            ItemObject.MakeInstance(drop.itemData, amount, dropCell.position);
            dropCell.inUse = true;
        }
        harvester.RemoveFromHarvestQueue(this);
        Destroy(this.gameObject);
    }
    void Start()
    {
        if (occupiedCell == null)
            Init();


    }
    public void Init()
    {
        RaycastHit gridHit;
        Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, out gridHit, 3f, LayerManager.instance.GroundLayerMask);
        Debug.Log(gridHit.transform.name);
        occupiedCell = GridManager.instance.GetCellFromPosition(gridHit.point);
        occupiedCell.inUse = true;
        occupiedCell.Walkable = false;
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
}
