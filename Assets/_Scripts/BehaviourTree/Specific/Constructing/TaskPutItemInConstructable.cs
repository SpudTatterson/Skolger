using BehaviorTree;
using UnityEngine.AI;

public class TaskPutItemInConstructable : Node
{
    NavMeshAgent agent;
    ColonistData colonistData;
    public TaskPutItemInConstructable(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var constructable = (IConstructable)GetData(DataName.Constructable);
        var item = (InventoryItem)GetData(DataName.InventoryItem);

        if (constructable != null && ColonistUtility.ReachedDestinationOrGaveUp(agent))
        {
            int itemIndex = (int)GetData(DataName.InventoryIndex);
            constructable.AddItem(colonistData.TakeItemOut(itemIndex));
            ClearData(DataName.InventoryItem);
            ClearData(DataName.Target);
            ClearData(DataName.Cost);

            if(constructable.CheckIfCostsFulfilled())
            {
                constructable.ConstructBuilding();
                ClearData(DataName.Constructable);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}