using BehaviorTree;
using UnityEngine;

public class CheckInPickupRange : Node
{
    public CheckInPickupRange() { }

    public override NodeState Evaluate()
    {
        parent.parent.ClearData("Target");
        Debug.Log("Target Was Collected");

        state = NodeState.SUCCESS;
        return state;
    }
}
