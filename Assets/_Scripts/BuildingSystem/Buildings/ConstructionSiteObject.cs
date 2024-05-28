using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSiteObject : MonoBehaviour, IConstructable, ISelectable, IAllowable
{
    public BuildingData buildingData { get; private set; }
    List<ItemCost> costs = new List<ItemCost>();
    SerializableDictionary<ItemData, int> fulfilledCosts = new SerializableDictionary<ItemData, int>();
    List<Cell> occupiedCells = new List<Cell>();
    bool allowed = true;
    // should probably hold ref to colonist that is supposed to build incase of canceling action + so that there wont be 2 colonists working on the same thing

    public void Initialize(BuildingData buildingData, List<Cell> occupiedCells)
    {
        this.buildingData = buildingData;
        this.occupiedCells = occupiedCells;

        foreach (ItemCost cost in this.buildingData.costs)
        {
            costs.Add(cost);
            if (fulfilledCosts.ContainsKey(cost.item))
                continue;
            else
                fulfilledCosts.Add(cost.item, 0);
        }
    }

    #region Construction

    public void AddItem(ItemObject itemObject)
    {
        fulfilledCosts[itemObject.itemData] += itemObject.amount;
        costs.RemoveAt(0);

        itemObject.UpdateAmount(itemObject.amount);

        CheckIfCanConstruct();
    }
    public ItemCost GetNextCost()
    {
        if (costs.Count != 0)
            return costs[0];
        return null;
    }
    public List<ItemCost> GetAllCosts()
    {
        return costs;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void CheckIfCanConstruct()
    {
        if (costs.Count == 0)
        {
            ConstructBuilding();
        }
    }

    public void ConstructBuilding()
    {
        BuildingObject.MakeInstance(buildingData, this.transform.position, occupiedCells);
        Destroy(this);
        Destroy(this.gameObject);
    }
    [ContextMenu("CancelConstruction")]
    public void CancelConstruction()
    {
        foreach (Cell c in occupiedCells)
        {
            c.inUse = false;
        }
        foreach (KeyValuePair<ItemData, int> cost in fulfilledCosts)
        {
            int stackSize = cost.Key.stackSize;
            Cell cell;
            if (cost.Value > stackSize)
            {
                int costToDisperse = cost.Value;
                while (costToDisperse > stackSize)
                {
                    cell = occupiedCells[0].GetClosestEmptyCell();

                    ItemObject.MakeInstance(cost.Key, stackSize, cell.position);
                    cell.inUse = true;
                    costToDisperse -= stackSize;
                }
                cell = occupiedCells[0].GetClosestEmptyCell();
                ItemObject.MakeInstance(cost.Key, costToDisperse, cell.position);
                cell.inUse = true;
            }
            else
            {
                cell = occupiedCells[0].GetClosestEmptyCell();
                ItemObject.MakeInstance(cost.Key, cost.Value, cell.position);
                cell.inUse = true;
            }
            Destroy(this.gameObject);
        }
    }

    #endregion

    public static ConstructionSiteObject MakeInstance(BuildingData buildingData, Cell cell, bool tempObject = false, Transform parent = null)
    {
        GameObject buildingGO = Instantiate(buildingData.buildingVisualUnplaced, cell.position, Quaternion.identity, parent);

        List<Cell> cells = new List<Cell>();
        if (!tempObject)
        {

            if (cell.grid.TryGetCells(new Vector2Int(cell.x, cell.y), buildingData.xSize, buildingData.ySize, out cells)
         && Cell.AreCellsFree(cells))
            {
                foreach (Cell c in cells)
                {
                    c.inUse = true;
                    Debug.Log(c.ToString() + " in use");
                }
            }
            else
            {
                Destroy(buildingGO);
                Debug.Log("destroying");
                return null;
            }
        }

        ConstructionSiteObject building = buildingGO.AddComponent<ConstructionSiteObject>();
        building.Initialize(buildingData, cells);


        return building;
    }

    #region Selection

    public SelectionType GetSelectionType()
    {
        return SelectionType.Constructable;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return buildingData.placeableName + "(Construction Site)";
    }

    public bool HasActiveCancelableAction()
    {
        return true;
    }

    #endregion

    #region IAllowable

    public void OnAllow()
    {
        allowed = true;
        // add to construction queue
        HaulerTest hauler = FindObjectOfType<HaulerTest>();
        hauler.AddConstructable(this);
    }

    public void OnDisallow()
    {
        allowed = false;
        // remove from construction queue
        HaulerTest hauler = FindObjectOfType<HaulerTest>();
        hauler.RemoveConstructable(this);
        // visually show that this is disallowed 
    }
    public bool IsAllowed()
    {
        return allowed;
    }


    #endregion
}
