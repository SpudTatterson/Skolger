using UnityEngine;
using BehaviorTree;

public class CheckForBed : Node
{
    ColonistData colonistData;
    public CheckForBed(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }
    public override NodeState Evaluate()
    {
        if (colonistData.restManger.assignedBed != null)
        {
            SetDataOnRoot(EDataName.Target, colonistData.restManger.assignedBed);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            if (BedManager.TryFindFreeBed(out BedBuilding bed))
            {
                colonistData.restManger.AssignBed(bed);
                bed.AssignColonist(colonistData);
                SetDataOnRoot(EDataName.Target, colonistData.restManger.assignedBed);
                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                // add to issues tracker that a colonist is missing a bed 

                BedBuilding tempBed = BuildingObject.MakeInstance(Resources.Load<BuildingData>("TempBed"),
                 GridManager.Instance.GetCellFromPosition(colonistData.transform.position).position,
                 Direction.TopLeft) as BedBuilding;

                colonistData.restManger.AssignBed(tempBed);
                tempBed.AssignColonist(colonistData);
                SetDataOnRoot(EDataName.Target, colonistData.restManger.assignedBed);
                if (BedManager.tempBeds.ContainsKey(colonistData))
                    BedManager.tempBeds[colonistData].Deconstruct();
                BedManager.tempBeds.Add(colonistData, tempBed);



                state = NodeState.RUNNING;
                return state;
            }
        }
    }
}