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
        if (HasSpace())
            PutItemIn(item.PickUp());
        if (heldItem.NullCheck())
        {
            Debug.LogWarning("Picked up null item");
            hauling = false;
            yield break;
        }

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
        if (!itemsToHaul.Contains(itemObject))
            itemsToHaul.Add(itemObject);
    }
    public void RemoveFromHaulQueue(ItemObject itemObject)
    {
        itemsToHaul.Remove(itemObject);
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
    public bool HasSpace()
    {
        return heldItem.NullCheck();
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

    public void PutItemIn(InventoryItem item)
    {
        item.OnDestroy += HandleItemDestruction;
        heldItem = item;
    }

    void HandleItemDestruction(InventoryItem item)
    {
        item.OnDestroy -= HandleItemDestruction;
        heldItem = null;
    }
}