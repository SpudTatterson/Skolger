using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHarvestable 
{
    public IEnumerator StartHarvesting();

    public bool IsBeingHarvested();
    public bool FinishedHarvesting();

    public List<ItemDrop> GetItemDrops();

    public void AddToHarvestQueue();
    public void RemoveFromHarvestQueue();
}
