using BehaviorTree;
using UnityEngine;

public class CheckForConstructableCost : Node 
{
    public override NodeState Evaluate()
    {
        IConstructable constructable = (IConstructable)GetData(DataName.Constructable);
        var cost = constructable.GetNextCost();

        if (cost != null)
        {
            SetDataOnRoot(DataName.Cost, cost);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}