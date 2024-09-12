using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CheckForStockpile : Node
{
    private NavMeshAgent agent;
    ColonistData colonistData;

    public CheckForStockpile(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        Stockpile stockpile = InventoryManager.Instance.GetStockpileWithEmptySpace(out Cell cell);
        InventoryItem inventoryItem = (InventoryItem)GetData(DataName.InventoryItem);
        
        if (cell == null)
        {
            object hasStockpile = GetData(DataName.Stockpile);

            if (inventoryItem != null || !colonistData.inventory.IsEmpty())
            {
                int itemIndex  = (int)GetData(DataName.InventoryIndex);
                colonistData.inventory.TakeItemOut(itemIndex).DropItem(agent.transform.position);
                agent.ResetPath();

                ClearData(DataName.InventoryItem);
            }

            if (hasStockpile != null)
            {
                agent.ResetPath();

                ClearData(DataName.Stockpile);
                ClearData(DataName.Cell);
            }

            state = NodeState.FAILURE;
            return state;
        }

        var target = GetData(DataName.Target);

        if (inventoryItem != null && target == null)
        {
            parent.parent.SetData(DataName.Target, cell);
        }

        parent.parent.SetData(DataName.Cell, cell);
        parent.parent.SetData(DataName.Stockpile, stockpile);

        state = NodeState.SUCCESS;
        return state;
    }
}