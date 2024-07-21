using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class BuilderTest : MonoBehaviour, IContainer<InventoryItem>
{
    [SerializeField, ReadOnly] InventoryItem heldItem;
    ItemCost costToGet;
    IConstructable constructable;
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
        if (!hauling && constructable == null)
        {
            constructable = TaskManager.Instance.PullConstructableFromQueue();
        }
        if (!hauling && constructable != null)
        {
            currentHaul = StartCoroutine(HaulItem(constructable));
        }
        if (hauling && constructable.SetForCancellation)
        {
            StopCoroutine(currentHaul);
            constructable = null;
            hauling = false;
        }
    }

    IEnumerator HaulItem(IConstructable constructable)
    {
        costToGet = constructable.GetNextCost();
        if (costToGet == null)
        {
            hauling = false;
            yield break;
        }

        hauling = true;

        if (InventoryManager.instance.HasItem(costToGet))
        {
            Vector3 itemPosition = InventoryManager.instance.GetItemLocation(costToGet.item, costToGet.cost);
            agent.SetDestination(itemPosition);
            while (!ReachedDestinationOrGaveUp())
            {
                yield return null;
            }
            if ((UnityEngine.Object)constructable == null)
            {
                costToGet = null;
                hauling = false;
                yield break;
            }
            PutItemIn(InventoryManager.instance.TakeItem(costToGet));// take item
            Vector3 constructablePosition = constructable.GetPosition(); // get constructable position
            agent.SetDestination(constructablePosition);
            while (!ReachedDestinationOrGaveUp())
            {
                yield return null;
            }

            constructable.AddItem(TakeItemOut(costToGet.item, costToGet.cost)); // add taken item

            hauling = false;
            heldItem = null;
            costToGet = null;
            this.constructable = null;

        }
        else
        {
            yield return new WaitForSeconds(3);
            hauling = false;
            Debug.Log("cant find enough items");
        }
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
