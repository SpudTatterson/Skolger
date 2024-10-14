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
        ItemObject item = (ItemObject)GetData(EDataName.Target);

        if(item != null && ReachedDestinationOrGaveUp())
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
    
    public bool ReachedDestinationOrGaveUp()
    {

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

}