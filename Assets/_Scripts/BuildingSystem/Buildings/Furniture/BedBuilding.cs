using UnityEngine;

public class BedBuilding : BuildingObject
{
    ColonistData assignedColonist;
    BedData data;

    void Awake()
    {
        data = buildingData as BedData;
    }
    public void SetAssignedColonist(ColonistData assignedColonist)
    {
        this.assignedColonist = assignedColonist;
    }

    public void Sleep(ColonistData colonist)
    {
        if (assignedColonist == null)
        {
            SetAssignedColonist(colonist);
        }
        if(colonist != assignedColonist)
        {
            Debug.Log($"{colonist.name} went to wrong bed!");
            return;
        }
        assignedColonist.restManger.Sleep(data.bedQuality);
    }
}