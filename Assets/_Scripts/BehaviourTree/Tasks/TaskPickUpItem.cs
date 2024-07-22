using BehaviorTree;
using UnityEngine.AI;
using UnityEngine;

public class TaskPickUpItem : Node
{
    private NavMeshAgent agent;

    public TaskPickUpItem(NavMeshAgent agent) 
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        ItemObject item = (ItemObject)GetData("Target");

        if(item != null && ReachedDestinationOrGaveUp())
        {
            InventoryItem inventoryItem = item.PickUp();

            parent.parent.parent.SetData("InventoryItem", inventoryItem);
            ClearData("Target");
            
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