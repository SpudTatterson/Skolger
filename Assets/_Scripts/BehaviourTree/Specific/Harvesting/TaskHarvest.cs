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
        object harvestableData = GetData(EDataName.Harvestable);
        IHarvestable harvestable = (IHarvestable)harvestableData;
        Vector3 harvestablePosition = ColonistUtility.ConvertToVector3(harvestableData);

        if (harvestable != null && !harvestable.IsBeingHarvested() && !harvestable.FinishedHarvesting() && ColonistUtility.ReachedDestination(agent, harvestablePosition))
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