using BehaviorTree;
using UnityEngine;

public class CheckItemInInventory : Node
{
    public CheckItemInInventory() {}

    public override NodeState Evaluate()
    {
        var inventoryItem = parent.parent.GetData("InventoryItem");
        
        if(inventoryItem == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}