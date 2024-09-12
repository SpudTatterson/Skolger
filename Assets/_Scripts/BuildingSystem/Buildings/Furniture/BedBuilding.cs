using System.Collections.Generic;
using UnityEngine;

public class BedBuilding : BuildingObject
{
    ColonistData assignedColonist;
    public BedData bedData { get; private set; }

    void Awake()
    {
        bedData = buildingData as BedData;
    }

    public override void Initialize(BuildingData buildingData, Direction placementDirection)
    {
        base.Initialize(buildingData, placementDirection);
        assignedColonist = null;
    }
    public void AssignColonist(ColonistData assignedColonist)
    {
        this.assignedColonist = assignedColonist;
    }

    public bool IsFree()
    {
        return assignedColonist == null;
    }

    protected override void OnDisable()
    {
        BedManager.Unsubscribe(this);
        assignedColonist?.restManger?.AssignBed(null);
        AssignColonist(null);
    }
    protected override void OnEnable()
    {
        BedManager.Subscribe(this);
    }


}

public class BedManager
{
    public static List<BedBuilding> bedBuildings = new List<BedBuilding>();

    public static void Subscribe(BedBuilding bed)
    {
        if (!bedBuildings.Contains(bed))
            bedBuildings.Add(bed);
    }
    public static void Unsubscribe(BedBuilding bed)
    {
        bedBuildings.Remove(bed);
    }

    public static bool TryFindFreeBed(out BedBuilding freeBed)
    {
        foreach (var bed in bedBuildings)
        {
            if (bed.IsFree())
            {
                freeBed = bed;
                return true;
            }
        }
        freeBed = null;
        return false;
    }
}
