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
            SetDataOnRoot(DataName.Target, colonistData.restManger.assignedBed);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            if (BedManager.TryFindFreeBed(out BedBuilding bed))
            {
                colonistData.restManger.AssignBed(bed);
                bed.AssignColonist(colonistData);
                SetDataOnRoot(DataName.Target, colonistData.restManger.assignedBed);
                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                // add to issues tracker that a colonist is missing a bed 
                state = NodeState.RUNNING;
                return state;
            }
        }
    }
}