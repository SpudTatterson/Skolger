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
        Transform target = (Transform)GetData("FollowTarget");

        if (Vector3.Distance(agent.transform.position, target.position) > 5)
        {
            agent.ResetPath();
            ClearData("FollowTarget");
        }

        if (Vector3.Distance(agent.transform.position, target.position) <= 5)
        {
            agent.SetDestination(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}