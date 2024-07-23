using BehaviorTree;
using UnityEngine;

public class CheckForHarvestable : Node
{
    public override NodeState Evaluate()
    {
        var hasHarvestable = GetData(DataName.Harvestable);

        if (hasHarvestable != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        var harvestable = TaskManager.Instance.PullHarvestableFromQueue();

        if (harvestable != null)
        {
            SetDataOnRoot(DataName.Harvestable, harvestable);
            SetDataOnRoot(DataName.Target, harvestable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}