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

        if (hasInventoryItem != null || !colonistData.IsEmpty())
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
            parent.parent.SetData(EDataName.Constructable, constructable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}