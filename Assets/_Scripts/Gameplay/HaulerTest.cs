using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class HaulerTest : MonoBehaviour//, IContainer<InventoryItem>
{
    [SerializeField, ReadOnly] InventoryItem heldItem;
    ItemObject itemToHaul;
    bool hauling = false;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (!hauling && itemToHaul == null)
        {
            itemToHaul = TaskManager.Instance.PullItemFromQueue();
            if (itemToHaul != null)
                StartCoroutine(HaulItem(itemToHaul));
        }
    }

    IEnumerator HaulItem(ItemObject item)
    {
        hauling = true;
        Stockpile stockpile = InventoryManager.Instance.GetStockpileWithEmptySpace(out Cell cell);
        if (cell == null)
        {
            Debug.Log("no space in stockpiles");
            yield return new WaitForSeconds(3);
            hauling = false;
            yield break;
        }
        agent.SetDestination(item.transform.position);
        while (!ColonistUtility.ReachedDestination(agent, item.transform.position))
        {
            yield return null;
        }
        if (HasSpace())
            PutItemIn(item.PickUp());
        if (heldItem.NullCheck())
        {
            Debug.LogWarning("Picked up null item");
            hauling = false;
            yield break;
        }

        agent.SetDestination(cell.position);
        while (!ColonistUtility.ReachedDestination(agent, item.transform.position))
        {
            yield return null;
        }
        if (stockpile.AddItem(heldItem))
        {
            heldItem = null;
        }
        else
        {
            Cell newCell = GridManager.Instance.GetCellFromPosition(agent.transform.position).GetClosestEmptyCell();
            heldItem.DropItem(newCell.position);
            newCell.inUse = true;
        }


        hauling = false;
        itemToHaul = null;
    }

    public bool HasItem(ItemData itemData, int amount)
    {
        if (itemData == heldItem.itemData && amount <= heldItem.amount) return true;
        return false;
    }
    public bool HasSpace()
    {
        if (heldItem == null || heldItem.NullCheck()) return true;
        else return false;
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