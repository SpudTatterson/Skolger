using BehaviorTree;
using UnityEngine;

public class CheckForEatable : Node
{
    public override NodeState Evaluate()
    {
        Debug.Log("Found Eatable");

        state = NodeState.SUCCESS;
        return state;
    }
}