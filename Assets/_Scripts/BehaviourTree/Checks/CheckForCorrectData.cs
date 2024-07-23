using System.Collections.Generic;
using BehaviorTree;

public class CheckForCorrectData : Node
{
    private List<DataName> dataName;

    public CheckForCorrectData(List<DataName> dataName)
    {
        this.dataName = dataName;
    }

    public override NodeState Evaluate()
    {        
        foreach(DataName data in dataName)
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