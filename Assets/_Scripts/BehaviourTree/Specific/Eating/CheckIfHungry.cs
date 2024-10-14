using BehaviorTree;
using UnityEngine;

public class CheckIfHungry : Node
{
    ColonistData colonistData;

    public CheckIfHungry(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        if(colonistData.hungerManager.IsHungry() && !colonistData.restManger.sleeping)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}