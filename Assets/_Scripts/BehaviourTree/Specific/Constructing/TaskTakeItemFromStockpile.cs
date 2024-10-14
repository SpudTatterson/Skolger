using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskTakeItemFromStockpile : Node
{
    NavMeshAgent agent;
    ColonistData colonistData;

    public TaskTakeItemFromStockpile(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var cost = (ItemCost)GetData(EDataName.Cost);

        if (cost != null && ReachedDestinationOrGaveUp())
        {
            Stockpile stockpile = (Stockpile)GetData(EDataName.Stockpile);
            var item = InventoryManager.Instance.TakeItem(cost, stockpile);
            parent.parent.SetData(EDataName.InventoryItem, item);
            colonistData.inventory.PutItemIn(item);
            var constructable = (IConstructable)GetData(EDataName.Constructable);
            ClearData(EDataName.Target);
            parent.parent.SetData(EDataName.Target, constructable.GetPosition());

            state = NodeState.SUCCESS;
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