using BehaviorTree;
using UnityEngine;

public class CheckForHaulable : Node
{
    public override NodeState Evaluate()
    {
        var haulable = TaskManager.Instance.PullItemFromQueue();

        if (haulable != null) 
        {
            parent.parent.SetData("Target", haulable);

            state = NodeState.SUCCESS;
            return state;
        }

        var data = parent.parent.GetData("Target");

        if(data != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
