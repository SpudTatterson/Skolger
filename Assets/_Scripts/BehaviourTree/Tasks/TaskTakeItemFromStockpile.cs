using BehaviorTree;
using UnityEngine;

public class TaskTakeItemFromStockpile : Node
{
    public override NodeState Evaluate()
    {
        var cost = (ItemCost)GetData("Cost");

        if (cost != null)
        {
            var item = InventoryManager.instance.TakeItem(cost);
            parent.parent.SetData("InventoryItem", item);
            var constructable = (IConstructable)GetData("Constructable");
            parent.parent.SetData("Target", constructable.GetPosition());

            Debug.Log("Cunt");
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}