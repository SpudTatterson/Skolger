using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class TaskManager : MonoSingleton<TaskManager>
{
    [ShowInInspector] UniqueQueue<IHarvestable> harvestQueue = new UniqueQueue<IHarvestable>();
    [ShowInInspector] UniqueQueue<IConstructable> constructionQueue = new UniqueQueue<IConstructable>();
    [ShowInInspector] List<ItemObject> haulQueue = new List<ItemObject>();

    public void AddToHarvestQueue(IHarvestable harvestable)
    {
        harvestQueue.Enqueue(harvestable);
    }
    public void RemoveFromHarvestQueue(IHarvestable harvestable)
    {
        harvestQueue.Remove(harvestable);
    }
    public IHarvestable PullHarvestableFromQueue()
    {
        if (harvestQueue.Count != 0)
            return harvestQueue.Dequeue();
        return null;
    }
    public bool CheckIfHarvestTaskExists()
    {
        return harvestQueue.Count > 0;
    }

    public void AddToConstructionQueue(IConstructable constructable)
    {
        constructionQueue.Enqueue(constructable);
    }
    public void RemoveFromConstructionQueue(IConstructable constructable)
    {
        constructionQueue.Remove(constructable);
    }
    public IConstructable PullConstructableFromQueue()
    {
        if (constructionQueue.Count != 0)
            return constructionQueue.Dequeue();
        return null;
    }

    public void AddToHaulQueue(ItemObject itemObject)
    {
        haulQueue.Add(itemObject);
    }
    public void RemoveFromHaulQueue(ItemObject itemObject)
    {
        haulQueue.Remove(itemObject);
    }
    public ItemObject PullItemFromQueue()
    {
        if (haulQueue.Count != 0)
        {
            ItemObject itemObject = haulQueue[0];
            haulQueue.RemoveAt(0);
            return itemObject;
        }
        return null;
    }
    public ItemObject PullItemFromQueue(NavMeshAgent agent)
    {
        if (CheckIfHaulTaskExists())
        {
            ItemObject closestItem = null;
            float closestDistance = Mathf.Infinity;

            foreach (var item in haulQueue)
            {
                if(!agent.CanReachPoint(item.transform.position)) continue;
                float distance = Vector3.Distance(item.transform.position, agent.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }

            if (closestItem != null)
            {
                haulQueue.Remove(closestItem);
                return closestItem;
            }
        }
        return null;
    }
    public bool CheckIfHaulTaskExists()
    {
        return haulQueue.Count > 0;
    }
}
