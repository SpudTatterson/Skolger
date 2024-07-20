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
        InventoryItem inventoryItem = (InventoryItem)GetData("InventoryItem");
        Stockpile stockpile = (Stockpile)GetData("Stockpile");

        if (inventoryItem == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (!ReachedDestinationOrGaveUp())
        {
            state = NodeState.FAILURE;
            return state;            
        }

        stockpile.AddItem(inventoryItem);
        
        ClearData("InventoryItem");
        ClearData("Cell");
        ClearData("Target");
        ClearData("Stockpile");

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