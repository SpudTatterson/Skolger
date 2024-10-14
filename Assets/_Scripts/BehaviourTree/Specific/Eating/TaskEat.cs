using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskEat : Node
{
    private NavMeshAgent agent;
    private ColonistData colonistData;

    public TaskEat(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        if (ReachedDestinationOrGaveUp())
        {
            EdibleData edibleData = (EdibleData)GetData(EDataName.FoodData);
            Stockpile stockpile = (Stockpile)GetData(EDataName.Stockpile);

            ClearData(EDataName.Target);
            ClearData(EDataName.Stockpile);
            ClearData(EDataName.FoodData);

            if (InventoryManager.Instance.HasItem(new ItemCost(edibleData, 1)))
            {
                IEdible edible = (EdibleInventoryItem)InventoryManager.Instance.TakeItem(new ItemCost(edibleData, 1), stockpile);
                colonistData.hungerManager.Eat(edible);
            }
            else
            {
                state = NodeState.FAILURE;
                return state;
            }

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