using System.Threading.Tasks;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskHarvest : Node
{
    NavMeshAgent agent;

    public TaskHarvest(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        IHarvestable harvestable = (IHarvestable)GetData("Harvestable");

        if (harvestable != null && !harvestable.IsBeingHarvested() && ReachedDestinationOrGaveUp())
        {
            TaskManager.Instance.StartCoroutine(harvestable.StartHarvesting());

            state = NodeState.RUNNING;
            return state;
        }

        if (harvestable.IsBeingHarvested())
        {
            if (harvestable.FinishedHarvesting())
            {
                Debug.Log("Im here");
                MonoBehaviour.Destroy(((MonoBehaviour)harvestable).gameObject);
                ClearData("Harvestable");
                ClearData("Target");

                state = NodeState.SUCCESS;
                return state;
            }
            state = NodeState.RUNNING;
            return state;
        }

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