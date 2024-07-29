using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskTakeItemFromStockpile : Node
{
    NavMeshAgent agent;
    ColonistData colonistData;

    public TaskTakeItemFromStockpile(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var cost = (ItemCost)GetData(EDataName.Cost);

        if (cost != null && ColonistUtility.ReachedDestinationOrGaveUp(agent))
        {
            Stockpile stockpile = (Stockpile)GetData(EDataName.Stockpile);
            var item = InventoryManager.instance.TakeItem(cost, stockpile);
            parent.parent.SetData(EDataName.InventoryItem, item);
            colonistData.PutItemIn(item);
            var constructable = (IConstructable)GetData(EDataName.Constructable);
            parent.parent.SetData(EDataName.Target, constructable.GetPosition());

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}