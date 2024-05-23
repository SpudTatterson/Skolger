using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnplacedPlaceableObject : MonoBehaviour, IConstructable
{
    public BuildingData buildingData { get; private set; }
    List<ItemCost> costs = new List<ItemCost>();
    SerializableDictionary<ItemData, int> fulfilledCosts = new SerializableDictionary<ItemData, int>();
    List<Cell> occupiedCells = new List<Cell>();


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

    [ContextMenu("ConstructBuilding")]
    public void ConstructBuilding()
    {
        BuildingObject.MakeInstance(buildingData, this.transform.position, occupiedCells);
        Destroy(this.gameObject);
    }

    public static UnplacedPlaceableObject MakeInstance(BuildingData buildingData, Cell cell, bool tempObject = false, Transform parent = null)
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

        UnplacedPlaceableObject building = buildingGO.AddComponent<UnplacedPlaceableObject>();
        building.Initialize(buildingData, cells);


        return building;
    }

}
