using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] BuildingData buildingData;
    [SerializeField] float timeForDragStart = 0.1f;
    bool placing;
    PlacementType placementType;
    float dragTime = 0;
    GameObject tempGO;
    List<GameObject> tempObjects = new List<GameObject>();
    Cell firstCell;
    Cell lastCell;
    List<GameObject> placedBuildings = new List<GameObject>();
    void Update()
    {
        if (Canceling())
            CancelPlacement();
        if (placing && buildingData != null && !EventSystem.current.IsPointerOverGameObject())
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerManager.instance.GroundLayerMask))
            {
                Cell hitCell = GridManager.instance.GetCellFromPosition(hit.point);

                if (tempGO == null)
                    InitializeNewPlacement(hitCell);

                tempGO.transform.position = hitCell.position;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    firstCell = hitCell;
                }
                else if (Input.GetKey(KeyCode.Mouse0))
                {
                    dragTime += Time.deltaTime;

                    if (lastCell != hitCell)
                    {
                        (Vector2Int cellAmount, Cell cornerCell) = GridObject.GetGridLineFrom2Cells(firstCell, hitCell);

                        hitCell.grid.TryGetCells((Vector2Int)cornerCell, cellAmount.x, cellAmount.y, out List<Cell> cells);

                        foreach (GameObject gameObject in tempObjects)
                        {
                            Destroy(gameObject);
                        }
                        tempObjects.Clear();
                        foreach (Cell cell in cells)
                        {
                            tempObjects.Add(ConstructionSiteObject.MakeInstance(buildingData, cell, true).gameObject);
                        }
                    }
                    lastCell = hitCell;
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0) && !hitCell.inUse && dragTime < timeForDragStart)
                {
                    PlaceBuilding(hitCell);
                    SelectionManager.instance.isSelecting = true;
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0) && dragTime > timeForDragStart)
                {
                    (Vector2Int cellAmount, Cell cornerCell) = GridObject.GetGridLineFrom2Cells(firstCell, hitCell);

                    hitCell.grid.TryGetCells((Vector2Int)cornerCell, cellAmount.x, cellAmount.y, out List<Cell> cells);
                    foreach (Cell cell in cells)
                    {
                        PlaceBuilding(cell);
                    }
                    SelectionManager.instance.isSelecting = true;
                }


            }
        }
    }

    void PlaceBuilding(Cell cell)
    {
        if (!cell.IsFreeAndExists()) return;
        ConstructionSiteObject building = ConstructionSiteObject.MakeInstance(buildingData, cell);
        placedBuildings.Add(building.gameObject);

        //hauler testing
        BuilderTest hauler = FindObjectOfType<BuilderTest>();
        hauler.AddConstructable(building);
        foreach (GameObject gameObject in tempObjects)
        {
            Destroy(gameObject);
        }
        tempObjects.Clear();
        Destroy(tempGO);
    }

    private void InitializeNewPlacement(Cell hitCell)
    {
        tempGO = ConstructionSiteObject.MakeInstance(buildingData, hitCell, true).gameObject;
        SelectionManager.instance.isSelecting = false;
    }

    public void SetNewBuilding(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        placementType = buildingData.placementType;
        placing = true;
    }
    public void CancelPlacement()
    {
        placing = false;
        buildingData = null;
        Destroy(tempGO);
        SelectionManager.instance.isSelecting = true;
    }
    bool Canceling()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1);
    }
}
