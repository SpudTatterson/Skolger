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
            parent.SetData(DataName.Harvestable, harvestable);
            parent.SetData(DataName.Target, harvestable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}