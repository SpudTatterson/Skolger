using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private NavMeshAgent agent;

    public TaskGoToTarget(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (Vector3.Distance(agent.transform.position, target.position) > 5)
        {
            agent.ResetPath();
            ClearData("Target");
        }

        if (Vector3.Distance(agent.transform.position, target.position) <= 5)
        {
            agent.SetDestination(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}