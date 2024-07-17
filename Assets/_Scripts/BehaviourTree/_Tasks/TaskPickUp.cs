using BehaviorTree;
using UnityEngine.AI;
using UnityEngine;

public class TaskPickUpItem : Node
{
    public TaskPickUpItem() {}

    public override NodeState Evaluate()
    {
        ItemObject item = (ItemObject)GetData("Target");

        if(item != null)
        {
            InventoryItem inventoryItem = item.PickUp();

            parent.parent.SetData("InventoryItem", inventoryItem);
            ClearData("Target");
            
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

}