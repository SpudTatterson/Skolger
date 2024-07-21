using BehaviorTree;
using UnityEngine;

public class CheckForConstructable : Node
{
    public override NodeState Evaluate()
    {
        var hasConstructable = GetData("Constructable");

        if (hasConstructable != null)
        {

            state = NodeState.SUCCESS;
            return state;
        }

        var constructable = TaskManager.Instance.PullConstructableFromQueue();

        if (constructable != null)
        {
            parent.parent.SetData("Constructable", constructable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}