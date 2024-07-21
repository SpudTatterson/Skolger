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
        var inventoryItem = GetData("InventoryItem");

        if (inventoryItem != null)
        {
            Debug.Log("failed first");
            state = NodeState.FAILURE;
            return state;
        }

        var hasTarget = GetData("Target");

        if(hasTarget != null && hasTarget is MonoBehaviour)
        {
                    Debug.Log("Able To haul");
            state = NodeState.SUCCESS;
            return state;
        }

        var haulable = TaskManager.Instance.PullItemFromQueue(agent.transform);

        if (haulable != null) 
        {
            parent.parent.SetData("Target", haulable);

        Debug.Log("Able To haul");
            state = NodeState.SUCCESS;
            return state;
        }

Debug.Log("failed last");
        state = NodeState.FAILURE;
        return state;
    }
}
