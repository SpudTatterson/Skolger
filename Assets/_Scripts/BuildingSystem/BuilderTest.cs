using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuilderTest : MonoBehaviour
{
    List<IConstructable> constructionQueue = new List<IConstructable>();
    InventoryItem heldItems;
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
            Vector3 itemPosition = InventoryManager.instance.GetItemLocation(costToGet.item, costToGet.cost);
            agent.SetDestination(itemPosition);
            while (!ReachedDestinationOrGaveUp())
            {
                // go to stockpile item
                //transform.Translate(VectorUtility.GetDirection(transform.position, itemPosition) * speed * Time.deltaTime);
                yield return null;
            }
            if ((UnityEngine.Object)constructable == null)
            {
                constructionQueue.Remove(constructable);
                hauling = false;
                yield break;
            }
            heldItems = InventoryManager.instance.TakeItem(costToGet);// take item
            Vector3 constructablePosition = constructable.GetPosition(); // get constructable position
            agent.SetDestination(constructablePosition);
            while (!ReachedDestinationOrGaveUp())
            {
                // go to constructable
                // transform.Translate(VectorUtility.GetDirection(transform.position, constructablePosition) * speed * Time.deltaTime);
                yield return null;
            }

            constructable.AddItem(heldItems); // add taken item

            hauling = false;
            heldItems = null;

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
}
