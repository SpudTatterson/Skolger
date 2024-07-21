using BehaviorTree;
using UnityEngine;

public class CheckForConstructableCost : Node 
{
    public override NodeState Evaluate()
    {
        IConstructable constructable = (IConstructable)GetData("Constructable");
        var cost = constructable.GetNextCost();

        if (cost != null)
        {
            parent.parent.SetData("Cost", cost);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}