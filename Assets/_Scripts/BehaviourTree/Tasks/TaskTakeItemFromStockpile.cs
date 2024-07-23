using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskTakeItemFromStockpile : Node
{
    NavMeshAgent agent;

    public TaskTakeItemFromStockpile(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        var cost = (ItemCost)GetData(DataName.Cost);

        if (cost != null && ReachedDestinationOrGaveUp())
        {
            Stockpile stockpile = (Stockpile)GetData(DataName.Stockpile);
            var item = InventoryManager.instance.TakeItem(cost, stockpile);
            SetDataOnRoot(DataName.InventoryItem, item);
            var constructable = (IConstructable)GetData(DataName.Constructable);
            SetDataOnRoot(DataName.Target, constructable.GetPosition());

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