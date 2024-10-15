using BehaviorTree;
using UnityEngine.AI;
using UnityEngine;

public class TaskPickUpItem : Node
{
    private NavMeshAgent agent;
    ColonistData colonistData;

    public TaskPickUpItem(NavMeshAgent agent, ColonistData colonistData) 
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        object itemData = GetData(EDataName.Target);
        ItemObject item = (ItemObject)itemData;
        Vector3 itemPosition = ColonistUtility.ConvertToVector3(itemData);

        if(item != null && ColonistUtility.ReachedDestination(agent, itemPosition))
        {
            InventoryItem inventoryItem = item.PickUp();

            parent.parent.SetData(EDataName.InventoryItem, inventoryItem);
            ClearData(EDataName.Target);
            colonistData.inventory.PutItemIn(inventoryItem);
            SetDataOnRoot(EDataName.InventoryIndex, inventoryItem.currentInventorySlot);
            
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}