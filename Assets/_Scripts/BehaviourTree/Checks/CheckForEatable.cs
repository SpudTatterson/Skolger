using BehaviorTree;
using UnityEngine;

public class CheckForEatable : Node
{
    public override NodeState Evaluate()
    {
        var food = GetData(DataName.FoodData);
        if(food != null)
        {
            state = NodeState.RUNNING;
            return state;
        }
        if (InventoryManager.Instance.TryFindFoodInStockpiles(out EdibleData edible, out Stockpile stockpile, out Cell itemPosition))
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