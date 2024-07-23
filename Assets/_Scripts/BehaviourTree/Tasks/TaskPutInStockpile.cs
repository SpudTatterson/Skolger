using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskPutInStockpile : Node
{
    private NavMeshAgent agent;

    public TaskPutInStockpile(NavMeshAgent agent) 
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        InventoryItem inventoryItem = (InventoryItem)GetData(DataName.InventoryItem);
        Stockpile stockpile = (Stockpile)GetData(DataName.Stockpile);

        if (inventoryItem == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (!ReachedDestinationOrGaveUp())
        {
            state = NodeState.RUNNING;
            return state;            
        }

        stockpile.AddItem(inventoryItem);

        ClearData(DataName.InventoryItem);
        ClearData(DataName.Cell);
        ClearData(DataName.Target);
        ClearData(DataName.Stockpile);

        state = NodeState.SUCCESS;
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