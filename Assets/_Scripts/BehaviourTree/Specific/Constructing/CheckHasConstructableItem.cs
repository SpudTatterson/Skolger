using BehaviorTree;
using UnityEngine;

public class CheckHasConstructableItem : Node
{
    public override NodeState Evaluate()
    {
        ItemCost itemCost = (ItemCost)GetData(EDataName.Cost);

        if (itemCost != null)
        {
            if (InventoryManager.Instance.HasItem(itemCost))
            {
                Cell itemPosition = InventoryManager.Instance.GetItemLocation(itemCost.item, itemCost.cost, out Stockpile stockpile);
                parent.parent.SetData(EDataName.Target, itemPosition);
                parent.parent.SetData(EDataName.Stockpile, stockpile);

                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}