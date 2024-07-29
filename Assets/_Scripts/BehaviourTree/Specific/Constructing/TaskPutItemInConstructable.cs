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
        var constructable = (IConstructable)GetData(EDataName.Constructable);
        var item = (InventoryItem)GetData(EDataName.InventoryItem);

        if (constructable != null && ColonistUtility.ReachedDestinationOrGaveUp(agent))
        {
            int itemIndex = (int)GetData(EDataName.InventoryIndex);
            constructable.AddItem(colonistData.TakeItemOut(itemIndex));
            ClearData(EDataName.InventoryItem);
            ClearData(EDataName.Target);
            ClearData(EDataName.Cost);

            if(constructable.CheckIfCostsFulfilled())
            {
                constructable.ConstructBuilding();
                ClearData(EDataName.Constructable);
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}