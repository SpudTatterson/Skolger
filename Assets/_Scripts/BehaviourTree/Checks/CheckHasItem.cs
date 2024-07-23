using BehaviorTree;
using UnityEngine;

public class CheckHasItem : Node
{
    public override NodeState Evaluate()
    {
        ItemCost itemCost = (ItemCost)GetData(DataName.Cost);

        if (itemCost != null)
        {
            if (InventoryManager.instance.HasItem(itemCost))
            {
                Cell itemPosition = InventoryManager.instance.GetItemLocation(itemCost.item, itemCost.cost, out Stockpile stockpile);
                SetDataOnRoot(DataName.Target, itemPosition);
                SetDataOnRoot(DataName.Stockpile, stockpile);

                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}