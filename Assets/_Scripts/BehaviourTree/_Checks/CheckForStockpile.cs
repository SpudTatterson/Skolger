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
        InventoryItem inventoryItem = (InventoryItem)GetData("InventoryItem");
        
        if (cell == null)
        {
            if (inventoryItem != null)
            {
                inventoryItem.DropItem(agent.transform.position);
                agent.ResetPath();

                ClearData("Target");
                ClearData("InventoryItem");
                ClearData("Cell");
                ClearData("Stockpile");
            }

            state = NodeState.FAILURE;
            return state;
        }

        if (inventoryItem != null)
        {
            parent.parent.SetData("Target", cell);
        }

        parent.parent.SetData("Cell", cell);
        parent.parent.SetData("Stockpile", stockpile);

        state = NodeState.SUCCESS;
        return state;
    }
}