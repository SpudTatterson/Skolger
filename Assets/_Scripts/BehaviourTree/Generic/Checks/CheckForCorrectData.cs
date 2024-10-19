using System;
using System.Collections.Generic;
using BehaviorTree;

public class CheckForCorrectData : Node
{
    private List<Enum> dataTypes;

    public CheckForCorrectData(List<Enum> data)
    {
        dataTypes = data;
    }

    public override NodeState Evaluate()
    {        
        foreach(Enum data in dataTypes)
        {
            var dataType = GetData(data);

            if (dataType == null)
            {
                state = NodeState.FAILURE;
                return state;
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}