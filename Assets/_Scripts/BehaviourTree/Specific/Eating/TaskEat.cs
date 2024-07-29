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
        if (ColonistUtility.ReachedDestinationOrGaveUp(agent))
        {
            EdibleData edibleData = (EdibleData)GetData(EDataName.FoodData);
            Stockpile stockpile = (Stockpile)GetData(EDataName.Stockpile);
            IEdible edible = (EdibleInventoryItem)InventoryManager.instance.TakeItem(new ItemCost(edibleData, 1), stockpile);
            colonistData.Eat(edible);

            ClearData(EDataName.Target);
            ClearData(EDataName.Stockpile);
            ClearData(EDataName.FoodData);
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}