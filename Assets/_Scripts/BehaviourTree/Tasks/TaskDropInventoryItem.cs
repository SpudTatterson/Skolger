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
        var inventoryItem = (InventoryItem)GetData(DataName.InventoryItem);

        if (inventoryItem != null || !colonistData.inventory.IsEmpty())
        {
            //inventoryItem.DropItem(agent.transform.position);
            object index = GetData(DataName.InventoryIndex);
            int InventoryIndex;
            if (index != null)
            {
                InventoryIndex = (int)index;
                colonistData.inventory.TakeItemOut(InventoryIndex).DropItem(GridManager.Instance.GetCellFromPosition(agent.transform.position).GetClosestEmptyCell().position);
                ClearData(DataName.InventoryItem);
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}