using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;

public class CheckForStockpile : Node
{
    public CheckForStockpile() {}

    public override NodeState Evaluate()
    {
        Stockpile stockpile = InventoryManager.instance.GetStockpileWithEmptySpace(out Cell cell);
        
        if(cell == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        parent.parent.SetData("Cell", cell);
        parent.parent.SetData("Stockpile", stockpile);

        state = NodeState.SUCCESS;
        return state;
    }
}