using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class BuilderTest : MonoBehaviour, IContainer<InventoryItem>
{
    List<IConstructable> constructionQueue = new List<IConstructable>();
    [SerializeField, ReadOnly] InventoryItem heldItem;
    ItemCost costToGet;
    bool hauling;
    Coroutine currentHaul;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if (constructionQueue.Count > 0 && !hauling)
        {
            if ((UnityEngine.Object)constructionQueue[0] == null)
                constructionQueue.Remove(constructionQueue[0]);
            else
                currentHaul = StartCoroutine(HaulItem(constructionQueue[0]));
        }
    }

    IEnumerator HaulItem(IConstructable constructable)
    {
        costToGet = constructable.GetNextCost();
        if (costToGet == null)
        {
            constructionQueue.Remove(constructable);
            hauling = false;
            yield break;
        }

        hauling = true;

        if (InventoryManager.instance.HasItem(costToGet))
        {
            Cell itemPosition = InventoryManager.instance.GetItemLocation(costToGet.item, costToGet.cost);
            agent.SetDestination(itemPosition.position);
            while (!ReachedDestinationOrGaveUp())
            {
                yield return null;
            }
            if ((UnityEngine.Object)constructable == null)
            {
                constructionQueue.Remove(constructable);
                hauling = false;
                yield break;
            }
            PutItemIn(InventoryManager.instance.TakeItem(costToGet));// take item
            Cell constructablePosition = constructable.GetPosition(); // get constructable position
            agent.SetDestination(constructablePosition.position);
            while (!ReachedDestinationOrGaveUp())
            {
                yield return null;
            }

            constructable.AddItem(TakeItemOut(costToGet.item, costToGet.cost)); // add taken item

            hauling = false;
            heldItem = null;

        }
        else
        {
            yield return new WaitForSeconds(3);
            hauling = false;
            Debug.Log("cant find enough items");
        }
    }

    public void AddConstructable(IConstructable constructable)
    {
        if (!constructionQueue.Contains(constructable))
            constructionQueue.Add(constructable);
    }
    public void RemoveConstructable(IConstructable constructable)
    {
        constructionQueue.Remove(constructable);
    }

    public bool ReachedDestinationOrGaveUp()
    {

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool HasItem(ItemData itemData, int amount)
    {
        if (heldItem != null && itemData == heldItem.itemData && amount <= heldItem.amount) return true;
        return false;
    }
    public bool HasSpace()
    {
        if (heldItem == null || heldItem.NullCheck()) return true;
        else return false;
    }
    public void PutItemIn(InventoryItem item)
    {
        item.OnDestroy += HandleItemDestruction;
        heldItem = item;

    }
    public InventoryItem TakeItemOut(ItemData itemData, int amount)
    {
        if (HasItem(itemData, amount))
        {
            heldItem.UpdateAmount(-amount);
            return new InventoryItem(itemData, amount);
        }
        return null;
    }

    void HandleItemDestruction(InventoryItem item)
    {
        item.OnDestroy -= HandleItemDestruction;
        heldItem = null;
    }
}
