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
        var constructable = (IConstructable)GetData("Constructable");
        var item = (InventoryItem)GetData("InventoryItem");

        if (constructable != null && ReachedDestinationOrGaveUp())
        {
            constructable.AddItem(item);
            ClearData("InventoryItem");
            ClearData("Target");
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