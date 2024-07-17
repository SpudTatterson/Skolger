using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToStockpile : Node
{
    private NavMeshAgent agent;

    public TaskGoToStockpile(NavMeshAgent agent) 
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        InventoryItem inventoryItem = (InventoryItem)GetData("InventoryItem");
        Cell cell = (Cell)GetData("Cell");
        Stockpile stockpile = (Stockpile)GetData("Stockpile");

        if (inventoryItem == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        agent.SetDestination(cell.position);

        if (!ReachedDestinationOrGaveUp())
        {
            state = NodeState.FAILURE;
            return state;            
        }

        stockpile.AddItem(inventoryItem);
        
        ClearData("InventoryItem");
        ClearData("Cell");
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