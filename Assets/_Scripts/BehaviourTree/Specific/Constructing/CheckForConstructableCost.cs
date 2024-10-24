using BehaviorTree;
using UnityEngine;

public class CheckForConstructableCost : Node
{
    public override NodeState Evaluate()
    {
        IConstructable constructable = (IConstructable)GetData(EDataName.Constructable);

        if (constructable == null)
        {
            ClearData(EDataName.Constructable);
            state = NodeState.FAILURE;
            return state;
        }
        var cost = constructable.GetNextCost();

        if (cost != null)
        {
            parent.parent.SetData(EDataName.Cost, cost);

            state = NodeState.SUCCESS;
            return state;
        }
        // else
        // {
        //     ClearData(EDataName.Constructable);
        // }

        state = NodeState.FAILURE;
        return state;
    }
}