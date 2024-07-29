using BehaviorTree;
using UnityEngine.AI;

public class TaskDropInventoryItem : Node
{
    private NavMeshAgent agent;
    ColonistData colonistData;
    public TaskDropInventoryItem(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var inventoryItem = (InventoryItem)GetData(EDataName.InventoryItem);

        if (inventoryItem != null || !colonistData.IsEmpty())
        {
            //inventoryItem.DropItem(agent.transform.position);
            int InventoryIndex = (int)GetData(EDataName.InventoryIndex);
            colonistData.TakeItemOut(InventoryIndex).DropItem(GridManager.instance.GetCellFromPosition(agent.transform.position).GetClosestEmptyCell().position);
            ClearData(EDataName.InventoryItem);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}