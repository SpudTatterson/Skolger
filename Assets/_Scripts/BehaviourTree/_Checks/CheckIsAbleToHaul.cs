using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckIsAbleToHaul : Node
{
    private NavMeshAgent agent;

    public CheckIsAbleToHaul(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        var InventoryItem = parent.parent.GetData("InventoryItem");

        if (InventoryItem != null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        var hasTarget = parent.parent.GetData("Target");

        if(hasTarget != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        var haulable = TaskManager.Instance.PullItemFromQueue(agent.transform);

        if (haulable != null) 
        {
            parent.parent.SetData("Target", haulable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
