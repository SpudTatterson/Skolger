using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : MonoBehaviour, ISelectable
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

    #region ISelectable

    public SelectionType GetSelectionType()
    {
        return SelectionType.Building;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return buildingData.placeableName;
    }

    public bool HasActiveCancelableAction()
    {
        return false;
    }

    #endregion

}
