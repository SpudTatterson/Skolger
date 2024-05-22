using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    public BuildingData buildingData { get; private set; }
    List<Cell> occupiedCells;

    public void Initialize(BuildingData buildingData, List<Cell> occupiedCells)
    {
        this.buildingData = buildingData;
        this.occupiedCells = occupiedCells;

        foreach (Cell cell in occupiedCells)
        {
            cell.Walkable = buildingData.walkable;
            cell.inUse = buildingData.takesFullCell;
        }
    }

    public static BuildingObject MakeInstance(BuildingData buildingData, Vector3 position,List<Cell> occupiedCells,  Transform parent = null)
    {
        GameObject buildingVisual = Instantiate(buildingData.buildingPrefab, position, Quaternion.identity, parent);

        BuildingObject building = buildingVisual.AddComponent<BuildingObject>();
        building.Initialize(buildingData, occupiedCells);

        return building;
    }
}
