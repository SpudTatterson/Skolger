using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] BuildingData buildingData;
    bool placing;
    GameObject tempGO;

    List<GameObject> placedBuildings = new List<GameObject>();
    void Update()
    {
        if (Canceling())
            CancelPlacement();
        if (placing && buildingData != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100f, LayerManager.instance.GroundLayerMask))
            {

                GridManager hitGrid = hit.transform.GetComponentInParent<GridManager>();

                Cell hitCell = hitGrid.GetCellFromPosition(hit.point);

                if(tempGO == null)
                    InitializeNewPlacement(hitCell);

                tempGO.transform.position = hitCell.position;

                if(Input.GetKeyDown(KeyCode.Mouse0) && !hitCell.inUse)
                {
                    UnplacedPlaceableObject building = UnplacedPlaceableObject.MakeInstance(buildingData, hitCell);
                    placedBuildings.Add(building.gameObject);
                }
            }
        }
    }

    private void InitializeNewPlacement(Cell hitCell)
    {
        tempGO = UnplacedPlaceableObject.MakeInstance(buildingData, hitCell, true).gameObject;
    }

    public void SetNewBuilding(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        placing = true;
    }
    public void CancelPlacement()
    {
        placing = false;
        buildingData = null;
        Destroy(tempGO);
    }
    bool Canceling()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1);
    }
}
