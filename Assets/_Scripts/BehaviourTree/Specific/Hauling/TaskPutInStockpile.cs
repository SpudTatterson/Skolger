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
        InventoryItem inventoryItem = (InventoryItem)GetData(EDataName.InventoryItem);
        Stockpile stockpile = (Stockpile)GetData(EDataName.Stockpile);

        if (inventoryItem == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (!ColonistUtility.ReachedDestinationOrGaveUp(agent))
        {
            state = NodeState.RUNNING;
            return state;            
        }

        int itemIndex = (int)GetData(EDataName.InventoryIndex);
        InventoryItem item = colonistData.Items[itemIndex];
        stockpile.AddItem(colonistData.TakeItemOut(itemIndex));
        ClearData(EDataName.InventoryItem);
        ClearData(EDataName.Cell);
        ClearData(EDataName.Target);
        ClearData(EDataName.Stockpile);
        state = NodeState.SUCCESS;
        return state;
    }
}