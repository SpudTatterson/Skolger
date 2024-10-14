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
                parent.parent.SetData(DataName.Target, itemPosition);
                parent.parent.SetData(DataName.Stockpile, stockpile);

                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}