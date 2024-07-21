using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskTakeItemFromStockpile : Node
{
    NavMeshAgent agent;

    public TaskTakeItemFromStockpile(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        var cost = (ItemCost)GetData("Cost");

        if (cost != null && ReachedDestinationOrGaveUp())
        {
            var item = InventoryManager.instance.TakeItem(cost);
            parent.parent.SetData("InventoryItem", item);
            var constructable = (IConstructable)GetData("Constructable");
            parent.parent.SetData("Target", constructable.GetPosition());

            Debug.Log(constructable.GetPosition());
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