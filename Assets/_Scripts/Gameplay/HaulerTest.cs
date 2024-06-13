using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HaulerTest : MonoBehaviour
{
    List<ItemObject> itemsToHaul = new List<ItemObject>();
    ItemObject heldItem;
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
        heldItem = item;
        if(heldItem == null) yield break;
        heldItem.gameObject.SetActive(false);

        agent.SetDestination(cell.position);
        while (!ReachedDestinationOrGaveUp())
        {
            yield return null;
        }
        RemoveFromHaulQueue(item);
        if (stockpile.AddItem(item))
        {
            Destroy(item.gameObject);
            heldItem = null;
        }
        else
        {
            Cell newCell = GridManager.instance.GetCellFromPosition(agent.transform.position).GetClosestEmptyCell();
            item.transform.position = newCell.position;
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
}