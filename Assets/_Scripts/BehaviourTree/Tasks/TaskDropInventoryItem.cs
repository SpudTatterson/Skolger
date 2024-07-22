using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskDropInventoryItem : Node
{
    private NavMeshAgent agent;

    public TaskDropInventoryItem(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {

        InventoryItem inventoryItem = (InventoryItem)GetData("InventoryItem");

        if (inventoryItem != null)
        {
            inventoryItem.DropItem(agent.transform.position);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}