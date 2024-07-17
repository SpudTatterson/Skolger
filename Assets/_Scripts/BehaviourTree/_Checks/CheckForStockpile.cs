using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;

public class CheckForStockpile : Node
{
    public CheckForStockpile() {}

    public override NodeState Evaluate()
    {
        InventoryManager.instance.GetStockpileWithEmptySpace(out Cell cell);
        
        if(cell == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        parent.parent.SetData("cell", cell);

        state = NodeState.SUCCESS;
        return state;
    }
}