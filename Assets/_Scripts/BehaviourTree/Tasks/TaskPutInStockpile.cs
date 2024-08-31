using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TaskPutInStockpile : Node
{
    private NavMeshAgent agent;
    ColonistData colonistData;

    public TaskPutInStockpile(NavMeshAgent agent, ColonistData colonistData) 
    {
        this.agent = agent;
        this.colonistData = colonistData;
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
        int itemIndex = (int)GetData(DataName.InventoryIndex);
        InventoryItem item = colonistData.inventory.Items[itemIndex];
        stockpile.AddItem(colonistData.inventory.TakeItemOut(itemIndex));
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