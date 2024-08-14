using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] BuildingData buildingData;
    [SerializeField] float timeForDragStart = 0.1f;
    bool placing;
    float dragTime = 0;
    GameObject tempGO;
    List<GameObject> tempObjects = new List<GameObject>();
    Cell firstCell;
    Cell lastCell;
    [SerializeField, ReadOnly] Direction placementDirection;
    [SerializeField, ReadOnly] List<GameObject> placedBuildings = new List<GameObject>();
    void Update()
    {
        if (Canceling() && placing)
            CancelPlacement();
        if (placing && buildingData != null && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                placementDirection = GetNextDirection(placementDirection);
                tempGO.transform.rotation = Quaternion.Euler(0, (int)placementDirection, 0);
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerManager.Instance.buildableLayerMask))
            {
                Cell hitCell = GridManager.Instance.GetCellFromPosition(hit.point);

                if (tempGO == null)
                    InitializeNewPlacement(hitCell);
                if (lastCell != hitCell)
                    tempGO.transform.position = hitCell.position;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstCell = hitCell;
                }
                else if (Input.GetKey(KeyCode.Mouse0))
                {
                    dragTime += Time.deltaTime;

                    if (lastCell != hitCell && lastCell != null)
                    {
                        List<Cell> cells = buildingData.PlacementStrategy.GetCells(firstCell, hitCell);
                        ReturnAllTemps();
                        foreach (Cell cell in cells)
                        {
                            tempObjects.Add(GenerateTempBuilding(cell));
                        }
                    }
                    lastCell = hitCell;
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0) && !hitCell.inUse && dragTime < timeForDragStart)
                {
                    PlaceBuilding(hitCell);
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0) && dragTime > timeForDragStart)
                {
                    List<Cell> cells = buildingData.PlacementStrategy.GetCells(firstCell, hitCell);
                    foreach (Cell cell in cells)
                    {
                        PlaceBuilding(cell);
                    }
                }


            }
        }
    }

    Direction GetNextDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.TopLeft: return Direction.TopRight;
            case Direction.TopRight: return Direction.BottomRight;
            case Direction.BottomRight: return Direction.BottomLeft;
            default: return Direction.TopLeft;
        }

    }

    void PlaceBuilding(Cell cell)
    {
        ReturnAllTemps();

        if ((cell.IsFree() && buildingData is not FloorTile) || (buildingData is FloorTile && !cell.hasFloor))
        {
            ConstructionSiteObject constructionSite = ConstructionSiteObject.MakeInstance(buildingData, cell, placementDirection);

            placedBuildings.Add(constructionSite.gameObject);

            TaskManager.Instance.AddToConstructionQueue(constructionSite);
        }
    }

    void ReturnAllTemps()
    {
        foreach (GameObject gameObject in tempObjects)
        {
            ReturnTemp(gameObject);
        }
        tempObjects.Clear();
    }

    void InitializeNewPlacement(Cell hitCell)
    {
        tempGO = GenerateTempBuilding(hitCell);
        SelectionManager.Instance.isSelecting = false;
    }
    void ReturnTemp(GameObject temp)
    {
        temp.layer = buildingData.unplacedVisual.layer;
        temp.transform.rotation = Quaternion.identity;
        PoolManager.Instance.ReturnObject(buildingData.unplacedVisual, temp);
    }
    GameObject GenerateTempBuilding(Cell hitCell)
    {
        GameObject temp = ConstructionSiteObject.MakeInstance(buildingData, hitCell, placementDirection, temp: true).gameObject;
        temp.transform.rotation = Quaternion.Euler(0, (float)placementDirection, 0);
        temp.layer = 0;
        return temp;
    }

    public void SetNewBuilding(BuildingData buildingData)
    {
        if (tempGO != null)
            CancelPlacement();
        this.buildingData = buildingData;
        placing = true;
    }
    public void CancelPlacement()
    {
        placing = false;
        ReturnAllTemps();
        ReturnTemp(tempGO);
        tempGO = null;
        firstCell = null;
        lastCell = null;
        buildingData = null;
        SelectionManager.Instance.isSelecting = true;
    }
    bool Canceling()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1);
    }
}
