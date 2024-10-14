using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskGoToSleep : Node
{
    ColonistData colonistData;
    NavMeshAgent agent;
    public TaskGoToSleep(ColonistData colonistData)
    {
        this.colonistData = colonistData;
        this.agent = colonistData.agent;
    }
    public override NodeState Evaluate()
    {
        if (ReachedDestinationOrGaveUp())
        {
            colonistData.restManger.Sleep();
            ClearData(EDataName.Target);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }
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