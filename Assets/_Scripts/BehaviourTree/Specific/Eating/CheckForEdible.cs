using BehaviorTree;
using UnityEngine;

public class CheckForEdible : Node
{
    public override NodeState Evaluate()
    {
        var food = GetData(EDataName.FoodData);
        
        if(food != null)
        {
            state = NodeState.RUNNING;
            return state;
        }
        if (InventoryManager.Instance.TryFindFoodInStockpiles(out EdibleData edible, out Stockpile stockpile, out Cell itemPosition))
        {
            SetDataOnRoot(EDataName.FoodData, edible);
            SetDataOnRoot(EDataName.Stockpile, stockpile);
            SetDataOnRoot(EDataName.Target, itemPosition);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}