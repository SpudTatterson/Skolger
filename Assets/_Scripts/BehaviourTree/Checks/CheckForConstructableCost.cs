using BehaviorTree;
using UnityEngine;

public class CheckForConstructableCost : Node
{
    public override NodeState Evaluate()
    {
        IConstructable constructable = (IConstructable)GetData(DataName.Constructable);

        if (constructable == null)
        {
            ClearData(DataName.Constructable);
            state = NodeState.FAILURE;
            return state;
        }
        var cost = constructable.GetNextCost();

        if (cost != null)
        {
            parent.parent.SetData(DataName.Cost, cost);

            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            ClearData(DataName.Constructable);
        }

        state = NodeState.FAILURE;
        return state;
    }
}