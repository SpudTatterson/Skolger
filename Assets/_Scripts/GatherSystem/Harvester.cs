using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour
{

    [SerializeField] List<IHarvestable> toHarvest = new List<IHarvestable>();
    Coroutine currentHarvesting;

    // Update is called once per frame
    void Update()
    {
        if (toHarvest.Count > 0 && !toHarvest[0].IsBeingHarvested())
        {
            currentHarvesting = StartCoroutine(toHarvest[0].StartHarvesting());
        }
    }

    public void AddToHarvestQueue(IHarvestable harvestable)
    {
        toHarvest.Add(harvestable);
    }

    public void RemoveFromHarvestQueue(IHarvestable harvestable)
    {
        if(toHarvest[0] == harvestable)
        {
            StopCoroutine(currentHarvesting);
        }
        toHarvest.Remove(harvestable);
    }
}
