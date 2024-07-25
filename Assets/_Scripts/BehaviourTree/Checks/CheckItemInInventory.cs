using BehaviorTree;
using UnityEngine;

public class CheckItemInInventory : Node
{
    ColonistData colonistData;
    public CheckItemInInventory(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var inventoryItem = GetData(DataName.InventoryItem);
        if (inventoryItem == null || colonistData.IsEmpty())
        {
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}