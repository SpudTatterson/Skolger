using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckHaulableInReach : Node
{
    private NavMeshAgent agent;

    public CheckHaulableInReach(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        var data = parent.parent.GetData("Target");

        if(data != null)
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
