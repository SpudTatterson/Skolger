using BehaviorTree;
using UnityEngine.AI;
using UnityEngine;

public class TaskPickUpItem : Node
{
    public TaskPickUpItem() {}

    public override NodeState Evaluate()
    {
        var item = GetData("Target");

        if(item != null)
        {
            Debug.Log("Item picked up");
            parent.parent.SetData("InventoryItem", item);
            ClearData("Target");
            
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
