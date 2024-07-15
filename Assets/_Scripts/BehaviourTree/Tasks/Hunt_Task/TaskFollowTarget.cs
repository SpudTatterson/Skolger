using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskFollowTarget : Node
{
    private NavMeshAgent agent;

    public TaskFollowTarget(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Target");

        if (Vector3.Distance(agent.transform.position, target.position) > 50)
        {
            agent.ResetPath();
            ClearData("Target");
        }

        if (Vector3.Distance(agent.transform.position, target.position) <= 50)
        {
            agent.SetDestination(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}