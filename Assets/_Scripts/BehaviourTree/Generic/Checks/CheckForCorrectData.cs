using System.Collections.Generic;
using BehaviorTree;

public class CheckForCorrectData : Node
{
    private List<EDataName> dataName;

    public CheckForCorrectData(List<EDataName> dataName)
    {
        this.dataName = dataName;
    }

    public override NodeState Evaluate()
    {        
        foreach(EDataName data in dataName)
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