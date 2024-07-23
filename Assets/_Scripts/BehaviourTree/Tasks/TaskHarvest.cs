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
        IHarvestable harvestable = (IHarvestable)GetData(DataName.Harvestable);

        if (harvestable != null && !harvestable.IsBeingHarvested() && !harvestable.FinishedHarvesting() && ReachedDestinationOrGaveUp())
        {
            TaskManager.Instance.StartCoroutine(harvestable.StartHarvesting());

            state = NodeState.RUNNING;
            return state;
        }

        if (harvestable.FinishedHarvesting())
        {
            Debug.Log("Im here");
            MonoBehaviour.Destroy(((MonoBehaviour)harvestable).gameObject);
            ClearData(DataName.Harvestable);
            ClearData(DataName.Target);

            state = NodeState.SUCCESS;
            return state;
        }

        if (harvestable.IsBeingHarvested())
        {
            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.RUNNING;
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