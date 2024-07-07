using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class HaulerTest : MonoBehaviour, IContainer<InventoryItem>
{
    List<ItemObject> itemsToHaul = new List<ItemObject>();
    [SerializeField, ReadOnly] InventoryItem heldItem;
    bool hauling = false;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (itemsToHaul.Count != 0 && !hauling)
        {
            Debug.Log("started Hauling");
            StartCoroutine(HaulItem(itemsToHaul[0]));
        }
    }

    IEnumerator HaulItem(ItemObject item)
    {
        hauling = true;
        Stockpile stockpile = InventoryManager.instance.GetStockpileWithEmptySpace(out Cell cell);
        if (cell == null)
        {
            Debug.Log("no space in stockpiles");
            yield return new WaitForSeconds(3);
            hauling = false;
            yield break;
        }
        agent.SetDestination(item.transform.position);
        while (!ReachedDestinationOrGaveUp())
        {
            yield return null;
        }
        RemoveFromHaulQueue(item);
        AddItem(item.PickUp());
        if (heldItem == null) yield break;

        agent.SetDestination(cell.position);
        while (!ReachedDestinationOrGaveUp())
        {
            yield return null;
        }
        if (stockpile.AddItem(heldItem))
        {
            heldItem = null;
        }
        else
        {
            Cell newCell = GridManager.instance.GetCellFromPosition(agent.transform.position).GetClosestEmptyCell();
            heldItem.DropItem(newCell.position);
            newCell.inUse = true;
        }


        hauling = false;
    }

    public void AddToHaulQueue(ItemObject itemObject)
    {
        Debug.Log("test");
        if (!itemsToHaul.Contains(itemObject))
            itemsToHaul.Add(itemObject);
    }
    public void RemoveFromHaulQueue(ItemObject itemObject)
    {
        itemsToHaul.Remove(itemObject);
        Debug.Log(itemsToHaul.Count);
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
        if (itemData == heldItem.itemData && amount <= heldItem.amount) return true;
        return false;
    }

    public InventoryItem TakeItem(ItemData itemData, int amount)
    {
        if (HasItem(itemData, amount))
        {
            heldItem.UpdateAmount(-amount);
            return new InventoryItem(itemData, amount);
        }
        return null;
    }

    public bool AddItem(InventoryItem item)
    {
        if (heldItem != null)
        {
            item.OnDestroy += HandleItemDestruction;
            heldItem = item;
            return true;
        }
        Debug.Log("no space");
        return false;
    }

    void HandleItemDestruction(InventoryItem item)
    {
        item.OnDestroy -= HandleItemDestruction;
        heldItem = null;
    }
}