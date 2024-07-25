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
        ItemObject item = (ItemObject)GetData(DataName.Target);

        if(item != null && ReachedDestinationOrGaveUp())
        {
            InventoryItem inventoryItem = item.PickUp();

            SetDataOnRoot(DataName.InventoryItem, inventoryItem);
            ClearData(DataName.Target);
            colonistData.PutItemIn(inventoryItem);
            parent.parent.parent.SetData(DataName.InventoryIndex, inventoryItem.currentInventorySlot);
            
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
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