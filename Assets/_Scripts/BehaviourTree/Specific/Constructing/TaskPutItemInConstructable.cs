using BehaviorTree;
using UnityEngine;
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
        var constructable = (IConstructable)GetData(EDataName.Constructable);
        var item = (InventoryItem)GetData(EDataName.InventoryItem);

        if (constructable != null && ReachedDestinationOrGaveUp())
        {
            var data = GetData(EDataName.InventoryIndex);
            if (data == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            int itemIndex = (int)data;
            constructable.AddItem(colonistData.inventory.TakeItemOut(itemIndex));
            ClearData(EDataName.InventoryItem);
            ClearData(EDataName.Target);
            ClearData(EDataName.Cost);

            if (constructable.CheckIfCostsFulfilled())
            {
                constructable.ConstructBuilding();
                ClearData(EDataName.Constructable);
                state = NodeState.SUCCESS;
                return state;
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