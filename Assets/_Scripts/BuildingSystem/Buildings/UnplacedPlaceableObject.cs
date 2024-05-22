using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnplacedPlaceableObject : MonoBehaviour, IConstructable
{
    public BuildingData buildingData { get; private set; }
    SerializableDictionary<ItemData, int> costs = new SerializableDictionary<ItemData, int>();
    SerializableDictionary<ItemData, int> fulfilledCosts = new SerializableDictionary<ItemData, int>();
    List<Cell> occupiedCells = new List<Cell>();


    public void Initialize(BuildingData buildingData, List<Cell> occupiedCells)
    {
        this.buildingData = buildingData;
        this.occupiedCells = occupiedCells;

        foreach (ItemCost cost in this.buildingData.costs)
        {
            costs.Add(cost.item, cost.cost);
            fulfilledCosts.Add(cost.item, 0);
        }
    }

    public void AddItem(ItemObject itemObject)
    {
        fulfilledCosts[itemObject.itemData] += itemObject.amount;
        costs[itemObject.itemData] -= itemObject.amount;
        if (costs[itemObject.itemData] == 0)
        {
            costs.Remove(itemObject.itemData);
        }
        itemObject.UpdateAmount(itemObject.amount);

        CheckIfCanConstruct();
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
        //set all cells in building size as in use
        //in building object set them as unwalkable

        UnplacedPlaceableObject building = buildingGO.AddComponent<UnplacedPlaceableObject>();
        building.Initialize(buildingData, cells);


        return building;
    }

}
