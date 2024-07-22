using BehaviorTree;
using UnityEngine;

public class CheckForHarvestable : Node
{
    public override NodeState Evaluate()
    {
        var hasHarvestable = GetData("Harvestable");

        if (hasHarvestable != null)
        {
            Debug.Log("Already has harvestable");

            state = NodeState.SUCCESS;
            return state;
        }

        var harvestable = TaskManager.Instance.PullHarvestableFromQueue();

        if (harvestable != null)
        {
            parent.parent.SetData("Harvestable", harvestable);
            parent.parent.SetData("Target", harvestable);
            Debug.Log("Harvestable target set");

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}