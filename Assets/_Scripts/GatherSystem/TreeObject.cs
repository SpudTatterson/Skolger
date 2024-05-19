using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : MonoBehaviour, IHarvestable
{
    [SerializeField] float baseGatherTime = 5f;

    [SerializeField] List<ItemDrop> drops = new List<ItemDrop>();
    float timeHarvesting = 0f;
    List<Cell> potentialDropPoints = new List<Cell>();
    Cell occupiedCell;
    bool beingHarvested = false;
    Harvester harvester;
    public void Harvest()
    {
        foreach (ItemDrop drop in drops)
        {
            int amount = Random.Range(drop.minDropAmount, drop.maxDropAmount);
            if (amount == 0) continue;
            ItemObject.MakeInstance(drop.itemData, amount, potentialDropPoints[0].position);
            potentialDropPoints.RemoveAt(0);
        }
        harvester.RemoveFromHarvestQueue(this);
        Destroy(this.gameObject);
    }
    void Start()
    {
        if (occupiedCell == null)
            Init();

        harvester = FindObjectOfType<Harvester>();
        harvester.AddToHarvestQueue(this);
    }
    public void Init()
    {
        // get occupied cell somehow
        RaycastHit gridHit;
        Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, out gridHit, 3f, LayerManager.instance.GroundLayerMask);
        Debug.Log(gridHit.transform.name);
        GridManager grid = gridHit.transform.GetComponentInParent<GridManager>();
        occupiedCell = grid.GetCellFromPosition(gridHit.point);
        occupiedCell.inUse = true;
        occupiedCell.Walkable = false;
        //add method to gridManager to get all cells adjacent to this cell
        grid.TryGetCells(new Vector2Int(occupiedCell.x, occupiedCell.y), 2, 2, out potentialDropPoints);
        //add them to potential cells
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

}
