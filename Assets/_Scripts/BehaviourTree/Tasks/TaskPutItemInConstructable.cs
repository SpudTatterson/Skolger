using BehaviorTree;
using UnityEngine.AI;

public class TaskPutItemInConstructable : Node
{
    NavMeshAgent agent;

    public TaskPutItemInConstructable(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        var constructable = (IConstructable)GetData(DataName.Constructable);
        var item = (InventoryItem)GetData(DataName.InventoryItem);

        if (constructable != null && ReachedDestinationOrGaveUp())
        {
            constructable.AddItem(item);
            ClearData(DataName.InventoryItem);
            ClearData(DataName.Target);
            ClearData(DataName.Cost);

            if(constructable.CheckIfCostsFulfilled())
            {
                constructable.ConstructBuilding();
                ClearData(DataName.Constructable);
            }

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