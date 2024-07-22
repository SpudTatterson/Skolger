using BehaviorTree;
using UnityEngine;

public class CheckForHarvestable : Node
{
    public override NodeState Evaluate()
    {
        TaskManager.Instance.PullHarvestableFromQueue();

        state = NodeState.FAILURE;
        return state;
    }
}