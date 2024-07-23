using BehaviorTree;
using UnityEngine;

public class CheckForConstructable : Node
{
    public override NodeState Evaluate()
    {
        var hasConstructable = GetData(DataName.Constructable);
        var hasInventoryItem = GetData(DataName.InventoryItem);

        if (hasInventoryItem != null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (hasConstructable != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        var constructable = TaskManager.Instance.PullConstructableFromQueue();

        if (constructable != null)
        {
            SetDataOnRoot(DataName.Constructable, constructable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}