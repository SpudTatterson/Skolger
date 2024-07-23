using System.Collections.Generic;
using BehaviorTree;

public class CheckForCorrectData : Node
{
    private List<string> dataName;

    public CheckForCorrectData(List<string> dataName)
    {
        this.dataName = dataName;
    }

    public override NodeState Evaluate()
    {        
        foreach(string data in dataName)
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