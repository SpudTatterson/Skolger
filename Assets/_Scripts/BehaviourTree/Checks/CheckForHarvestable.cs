using BehaviorTree;
using UnityEngine;

public class CheckForHarvestable : Node
{
    public override NodeState Evaluate()
    {
        var hasHarvestable = GetData("Harvestable");

        if (hasHarvestable != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        var harvestable = TaskManager.Instance.PullHarvestableFromQueue();

        if (harvestable != null)
        {
            parent.parent.SetData("Harvestable", harvestable);
            parent.parent.SetData("Target", harvestable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}