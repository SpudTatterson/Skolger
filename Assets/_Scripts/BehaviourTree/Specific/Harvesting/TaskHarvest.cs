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
        IHarvestable harvestable = (IHarvestable)GetData(EDataName.Harvestable);

        if (harvestable != null && !harvestable.IsBeingHarvested() && !harvestable.FinishedHarvesting() && ColonistUtility.ReachedDestinationOrGaveUp(agent))
        {
            TaskManager.Instance.StartCoroutine(harvestable.StartHarvesting());
        }

        if (harvestable.FinishedHarvesting())
        {
            MonoBehaviour.Destroy(((MonoBehaviour)harvestable).gameObject);
            ClearData(EDataName.Harvestable);
            ClearData(EDataName.Target);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}