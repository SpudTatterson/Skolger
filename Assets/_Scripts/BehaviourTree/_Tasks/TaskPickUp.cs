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
            item.gameObject.SetActive(false);
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
