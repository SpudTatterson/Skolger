using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    List<IHarvestable> harvestQueue = new List<IHarvestable>();
    List<IConstructable> constructionQueue = new List<IConstructable>();
    List<ItemObject> haulQueue = new List<ItemObject>();
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("More Than one task manager exists");
            Destroy(this);
        }
    }

    public void AddToHarvestQueue(IHarvestable harvestable)
    {
        if (!harvestQueue.Contains(harvestable))
            harvestQueue.Add(harvestable);
    }
    public void RemoveFromHarvestQueue(IHarvestable harvestable)
    {
        harvestQueue.Remove(harvestable);
    }
    public IHarvestable PullHarvestableFromQueue()
    {
        if (harvestQueue.Count > 0)
            return harvestQueue[0];
        return null;
    }

    public void AddToConstructionQueue(IConstructable constructable)
    {
        if (!constructionQueue.Contains(constructable))
            constructionQueue.Add(constructable);
    }
    public void RemoveFromConstructionQueue(IConstructable constructable)
    {
        constructionQueue.Remove(constructable);
    }
    public IConstructable PullConstructableFromQueue()
    {
        if (constructionQueue.Count > 0)
            return constructionQueue[0];
        return null;
    }

    public void AddToHaulQueue(ItemObject itemObject)
    {
        if (!haulQueue.Contains(itemObject))
            haulQueue.Add(itemObject);
        Debug.Log("test");
    }
    public void RemoveFromHaulQueue(ItemObject itemObject)
    {
        haulQueue.Remove(itemObject);
    }
    public ItemObject PullItemFromQueue()
    {
        if (haulQueue.Count > 0)
        {
            ItemObject item = haulQueue[0];
            haulQueue.RemoveAt(0);
            return item;
        }
        return null;
    }
    public ItemObject PullItemFromQueue(Transform transform)
    {
        if (haulQueue.Count > 0)
        {
            float distance = Mathf.Infinity;
            int index = 0;

            for (int i = 0; i <haulQueue.Count; i++)
            {
                float agentDistance = Vector3.Distance(haulQueue[i].transform.position, transform.position);
                if(distance > agentDistance)
                {
                    distance = agentDistance;
                    index = i;
                }
            }

            ItemObject item = haulQueue[index];
            haulQueue.RemoveAt(index);            
            return item;
        }

        return null;
    }
}
