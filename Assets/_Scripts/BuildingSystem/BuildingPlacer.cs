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
    [SerializeField, ReadOnly] List<GameObject> placedBuildings = new List<GameObject>();
    void Update()
    {
        if (Canceling() && placing)
            CancelPlacement();
        if (placing && buildingData != null && !EventSystem.current.IsPointerOverGameObject())
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerManager.instance.buildableLayerMask))
            {
                Cell hitCell = GridManager.instance.GetCellFromPosition(hit.point);

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
                        Debug.Log("test");
                        DestroyAllTemps();
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

    void PlaceBuilding(Cell cell)
    {
        if (cell.IsFree())
        {
            ConstructionSiteObject constructionSite = ConstructionSiteObject.MakeInstance(buildingData, cell);

            placedBuildings.Add(constructionSite.gameObject);

            TaskManager.Instance.AddToConstructionQueue(constructionSite);
        }

        DestroyAllTemps();
    }

    void DestroyAllTemps()
    {
        foreach (GameObject gameObject in tempObjects)
        {
            PoolManager.Instance.ReturnObject(buildingData.unplacedVisual, gameObject);
        }
        tempObjects.Clear();
    }

    void InitializeNewPlacement(Cell hitCell)
    {
        tempGO = GenerateTempBuilding(hitCell);
        SelectionManager.instance.isSelecting = false;
    }

    GameObject GenerateTempBuilding(Cell hitCell)
    {
        GameObject temp = ConstructionSiteObject.MakeInstance(buildingData, hitCell, temp: true).gameObject;
        temp.layer = 0;
        return temp;
    }

    public void SetNewBuilding(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        placing = true;
    }
    public void CancelPlacement()
    {
        placing = false;
        DestroyAllTemps();
        PoolManager.Instance.ReturnObject(buildingData.unplacedVisual, tempGO);
        tempGO = null;
        firstCell = null;
        lastCell = null;
        buildingData = null;
        SelectionManager.instance.isSelecting = true;
    }
    bool Canceling()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1);
    }
}
