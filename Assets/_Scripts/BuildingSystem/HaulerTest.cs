using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaulerTest : MonoBehaviour
{
    [SerializeField] float speed = 2.5f;
    List<IConstructable> constructionQueue = new List<IConstructable>();
    List<ItemObject> heldItems = new List<ItemObject>();
    ItemCost costToGet;
    bool hauling;

    // Update is called once per frame
    void Update()
    {
        
        if(constructionQueue.Count > 0 && !hauling)
        {
            StartCoroutine(HaulItem(constructionQueue[0]));
        }
    }

    IEnumerator HaulItem(IConstructable constructable)
    {
        costToGet = constructable.GetNextCost();
        if(costToGet == null) 
        {
            constructionQueue.Remove(constructable);
            hauling = false;
            yield break;
        }

        hauling = true;

        if(InventoryManager.instance.HasItem(costToGet))
        {
            List<Vector3> positions = InventoryManager.instance.GetItemLocations(costToGet.item, costToGet.cost);
            foreach (Vector3 position in positions)
            {
                while(Vector3.Distance(transform.position, position) > 1)
                {
                    // go to stockpile item
                    transform.Translate(VectorUtility.GetDirection(transform.position, position) * speed * Time.deltaTime);
                    yield return null;
                }
                heldItems = InventoryManager.instance.TakeItem(costToGet);// take item
                Vector3 constructablePosition = constructable.GetPosition(); // get constructable position
                while(Vector3.Distance(constructablePosition, transform.position) > 1)
                {
                    // go to constructable
                    transform.Translate(VectorUtility.GetDirection(transform.position, constructablePosition) * speed * Time.deltaTime);
                    yield return null;
                }
                foreach(ItemObject item in heldItems)
                {
                    constructable.AddItem(item); // add taken items
                }
                hauling = false;
                heldItems.Clear();

            }
        }
        else
        {
            hauling = false;
        }
        // find where can obtain item with correct amount
        // walk there
        // walk to constructable prob use a courntine for walking with distance check or something or check what cell he is in 
        // additem to constuctable

    }

    public void AddConstructable(IConstructable constructable)
    {
        constructionQueue.Add(constructable);
    }
    
}
