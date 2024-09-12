using UnityEngine;
using BehaviorTree;

public class CheckIfTired : Node
{
    ColonistData colonistData;
    public CheckIfTired(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }
    public override NodeState Evaluate()
    {
        if (colonistData.restManger.IsTired())
        {
            state = NodeState.SUCCESS;
            return state;
        }
        else
            state = NodeState.FAILURE;
        return state;
    }
}