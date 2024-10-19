using System;
using System.Collections.Generic;
using BehaviorTree;

public class CheckForCorrectData : Node
{
    private List<Enum> dataName;

    public CheckForCorrectData(List<Enum> dataName)
    {
        this.dataName = dataName;
    }

    public override NodeState Evaluate()
    {        
        foreach(Enum data in dataName)
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