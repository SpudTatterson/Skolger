using BehaviorTree;
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
        var inventoryItem = (InventoryItem)GetData("InventoryItem");

        if (inventoryItem != null)
        {
            inventoryItem.DropItem(agent.transform.position);
            ClearData("InventoryItem");
        }

        state = NodeState.SUCCESS;
        return state;
    }
}