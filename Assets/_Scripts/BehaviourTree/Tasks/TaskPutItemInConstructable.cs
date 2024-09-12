using BehaviorTree;
using UnityEngine.AI;

public class TaskPutItemInConstructable : Node
{
    NavMeshAgent agent;
    ColonistData colonistData;
    public TaskPutItemInConstructable(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var constructable = (IConstructable)GetData(DataName.Constructable);
        var item = (InventoryItem)GetData(DataName.InventoryItem);

        if (constructable != null && ReachedDestinationOrGaveUp())
        {
            var data = GetData(DataName.InventoryIndex);
            if (data == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            int itemIndex = (int)data;
            constructable.AddItem(colonistData.inventory.TakeItemOut(itemIndex));
            ClearData(DataName.InventoryItem);
            ClearData(DataName.Target);
            ClearData(DataName.Cost);

            if (constructable.CheckIfCostsFulfilled())
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