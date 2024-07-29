using BehaviorTree;
using UnityEngine;

public class CheckForEdible : Node
{
    public override NodeState Evaluate()
    {
        var food = GetData(DataName.FoodData);
        
        if(food != null)
        {
            state = NodeState.RUNNING;
            return state;
        }
        
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