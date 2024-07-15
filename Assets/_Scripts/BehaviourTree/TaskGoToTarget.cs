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
        Transform target = ((MonoBehaviour)GetData("Target")).transform;

        if (target != null)
        {
            agent.SetDestination(target.position);

            if (ReachedDestinationOrGaveUp())
            {
                Debug.Log("Succeeded");

                state = NodeState.SUCCESS;
                return state;
            }

            Debug.Log("Did not reach destination");
            state = NodeState.FAILURE;
            return state;
        }

        Debug.Log("Did not find a destination");
        state = NodeState.FAILURE;
        return state;
    }

    public bool ReachedDestinationOrGaveUp()
    {

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
