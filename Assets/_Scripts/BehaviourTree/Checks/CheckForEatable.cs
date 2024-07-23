using BehaviorTree;
using UnityEngine;

public class CheckForEatable : Node
{
    public override NodeState Evaluate()
    {

        if (InventoryManager.instance.TryFindFoodInStockpiles(out EdibleData edible, out Stockpile stockpile, out Cell itemPosition))
        {
            Debug.Log("Found Eatable");
            parent.parent.SetData("FoodData", edible);
            parent.parent.SetData("Stockpile", stockpile);
            parent.parent.SetData("Target", itemPosition);

            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            Debug.Log("Cant find food AHHH");
        }

        state = NodeState.FAILURE;
        return state;
    }
}