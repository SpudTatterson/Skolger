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
        var cost = (ItemCost)GetData(DataName.Cost);

        if (cost != null && ReachedDestinationOrGaveUp())
        {
            Stockpile stockpile = (Stockpile)GetData(DataName.Stockpile);
            var item = InventoryManager.instance.TakeItem(cost, stockpile);
            parent.parent.SetData(DataName.InventoryItem, item);
            colonistData.PutItemIn(item);
            var constructable = (IConstructable)GetData(DataName.Constructable);
            parent.parent.SetData(DataName.Target, constructable.GetPosition());

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