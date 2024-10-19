using System.Runtime.InteropServices.WindowsRuntime;
using BehaviorTree;
using UnityEngine;

public class CheckForBrainState : Node
{
    private EBrainState currentState;
    private EBrainState desiredState;

    public CheckForBrainState(EBrainState currentState, EBrainState desiredState)
    {
        this.currentState = currentState;
        this.desiredState = desiredState;
    }

    public override NodeState Evaluate()
    {
        if (currentState == desiredState)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}