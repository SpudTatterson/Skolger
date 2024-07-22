using BehaviorTree;
using UnityEngine;

public class CheckHasItem : Node
{
    public override NodeState Evaluate()
    {
        ItemCost itemCost = (ItemCost)GetData("Cost");

        if (itemCost != null)
        {
            if (InventoryManager.instance.HasItem(itemCost))
            {
                Cell itemPosition = InventoryManager.instance.GetItemLocation(itemCost.item, itemCost.cost);
                parent.parent.SetData("Target", itemPosition);
                
                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}