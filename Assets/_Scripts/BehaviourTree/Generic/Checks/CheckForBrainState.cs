using System.Runtime.InteropServices.WindowsRuntime;
using BehaviorTree;
using UnityEngine;

public class CheckForBrainState : Node
{
    private ColonistData colonistData;
    private EBrainState desiredState;

    public CheckForBrainState(ColonistData colonistData, EBrainState desiredState)
    {
        this.colonistData = colonistData;
        this.desiredState = desiredState;
    }
    
    public override NodeState Evaluate()
    {
        if (colonistData.brainState == desiredState)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}