using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoSingleton<TaskManager>
{

    HashSet<IHarvestable> harvestQueue = new HashSet<IHarvestable>();
    HashSet<IConstructable> constructionQueue = new HashSet<IConstructable>();
    HashSet<ItemObject> haulQueue = new HashSet<ItemObject>();

    public void AddToHarvestQueue(IHarvestable harvestable)
    {
        harvestQueue.Add(harvestable);
    }
    public void RemoveFromHarvestQueue(IHarvestable harvestable)
    {
        harvestQueue.Remove(harvestable);
    }
    public IHarvestable PullHarvestableFromQueue()
    {
        if (CheckIfHarvestTaskExists())
        {
            var enumerator = harvestQueue.GetEnumerator();
            enumerator.MoveNext();
            IHarvestable harvestable = enumerator.Current;
            harvestQueue.Remove(harvestable);
            return harvestable;
        }
        return null;
    }
    public bool CheckIfHarvestTaskExists()
    {
        return harvestQueue.Count > 0;
    }

    public void AddToConstructionQueue(IConstructable constructable)
    {
        constructionQueue.Add(constructable);
    }
    public void RemoveFromConstructionQueue(IConstructable constructable)
    {
        constructionQueue.Remove(constructable);
    }
    public IConstructable PullConstructableFromQueue()
    {
        if (CheckIfConstructableTaskExists())
        {
            var enumerator = constructionQueue.GetEnumerator();
            enumerator.MoveNext();
            IConstructable constructable = enumerator.Current;
            constructionQueue.Remove(constructable);
            return constructable;
        }
        return null;
    }
    public bool CheckIfConstructableTaskExists()
    {
        return constructionQueue.Count > 0;
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
        if (haulQueue.Count > 0)
        {
            var enumerator = haulQueue.GetEnumerator();
            enumerator.MoveNext();
            ItemObject item = enumerator.Current;
            haulQueue.Remove(item);
            return item;
        }
        return null;
    }
    public ItemObject PullItemFromQueue(Transform transform)
    {
        if (CheckIfHaulTaskExists())
        {
            ItemObject closestItem = null;
            float closestDistance = Mathf.Infinity;

            foreach (var item in haulQueue)
            {
                float distance = Vector3.Distance(item.transform.position, transform.position);
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
