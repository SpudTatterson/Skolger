using BehaviorTree;
using UnityEngine;

public class CheckForEatable : Node
{
    public override NodeState Evaluate()
    {

        if (InventoryManager.instance.TryFindFoodInStockpiles(out EdibleData edible, out Stockpile stockpile, out Cell itemPosition))
        {
            SetDataOnRoot(DataName.FoodData, edible);
            SetDataOnRoot(DataName.Stockpile, stockpile);
            SetDataOnRoot(DataName.Target, itemPosition);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}