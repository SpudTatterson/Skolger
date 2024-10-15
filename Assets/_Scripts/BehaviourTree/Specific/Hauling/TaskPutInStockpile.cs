using System.Runtime.InteropServices;
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
        object stockpileData = GetData(EDataName.Stockpile);
        object cell = GetData(EDataName.Target);
        Vector3 cellPosition = ColonistUtility.ConvertToVector3(cell); 
        Stockpile stockpile = (Stockpile)stockpileData;

        if (inventoryItem == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (!ColonistUtility.ReachedDestination(agent, cellPosition))
        {
            state = NodeState.RUNNING;
            return state;            
        }

        int itemIndex = (int)GetData(EDataName.InventoryIndex);
        InventoryItem item = colonistData.inventory.Items[itemIndex];
        stockpile.AddItem(colonistData.inventory.TakeItemOut(itemIndex));
        ClearData(EDataName.InventoryItem);
        ClearData(EDataName.Cell);
        ClearData(EDataName.Target);
        ClearData(EDataName.Stockpile);
        state = NodeState.SUCCESS;
        return state;
    }
}