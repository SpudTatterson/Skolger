using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CheckForStockpile : Node
{
    private NavMeshAgent agent;

    public CheckForStockpile(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        Stockpile stockpile = InventoryManager.instance.GetStockpileWithEmptySpace(out Cell cell);
        InventoryItem inventoryItem = (InventoryItem)GetData(DataName.InventoryItem);
        
        if (cell == null)
        {
            object hasStockpile = GetData(DataName.Stockpile);

            if (inventoryItem != null)
            {
                inventoryItem.DropItem(agent.transform.position);
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
            SetDataOnRoot(DataName.Target, cell);
        }

        SetDataOnRoot(DataName.Cell, cell);
        SetDataOnRoot(DataName.Stockpile, stockpile);

        state = NodeState.SUCCESS;
        return state;
    }
}