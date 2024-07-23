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
        var inventoryItem = GetData(DataName.InventoryItem);

        if (inventoryItem != null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        var hasTarget = GetData(DataName.Target);

        if(hasTarget != null && hasTarget is MonoBehaviour)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        var haulable = TaskManager.Instance.PullItemFromQueue(agent.transform);

        if (haulable != null) 
        {
            SetDataOnRoot(DataName.Target, haulable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
