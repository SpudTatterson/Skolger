using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckIsAbleToHaul : Node
{
    private NavMeshAgent agent;
    ColonistData colonistData;

    public CheckIsAbleToHaul(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var inventoryItem = GetData(DataName.InventoryItem);
        bool hasSpace = colonistData.HasSpace();

        if (inventoryItem != null || !hasSpace)
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
            parent.parent.SetData(DataName.Target, haulable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
