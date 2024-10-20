using BehaviorTree;
using UnityEngine;

public class CheckForConstructable : Node
{
    ColonistData colonistData;
    public CheckForConstructable(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }
    public override NodeState Evaluate()
    {
        var hasConstructable = GetData(EDataName.Constructable);
        var hasInventoryItem = GetData(EDataName.InventoryItem);

        if (hasInventoryItem != null || !colonistData.inventory.IsEmpty())
        {
            state = NodeState.RUNNING;
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
            if (colonistData.agent.CanReachPoint(constructable.GetPosition().position))
            {
                parent.parent.SetData(EDataName.Constructable, constructable);
                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                // mark constructable as unreachable and add to issue tracker
                TaskManager.Instance.AddToConstructionQueue(constructable);
                state = NodeState.FAILURE;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}