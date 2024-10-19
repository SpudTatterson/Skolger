using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckForHarvestable : Node
{
    NavMeshAgent agent;
    public CheckForHarvestable(NavMeshAgent agent)
    {
        this.agent = agent;
    }
    public override NodeState Evaluate()
    {
        var hasHarvestable = GetData(EDataName.Harvestable);

        if (hasHarvestable != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        var harvestable = TaskManager.Instance.PullHarvestableFromQueue();

        if (harvestable != null)
        {
            if (agent.CanReachPoint(ColonistUtility.ConvertToVector3(harvestable)))
            {
                parent.SetData(EDataName.Harvestable, harvestable);
                parent.SetData(EDataName.Target, harvestable);
                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                // add to issue tracker
                TaskManager.Instance.AddToHarvestQueue(harvestable);
                state = NodeState.FAILURE;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}