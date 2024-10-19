using BehaviorTree;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
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
        object constructableData = GetData(EDataName.Constructable);
        IConstructable constructable = (IConstructable)constructableData;
        Vector3 constructablePosition = ColonistUtility.ConvertToVector3(GetData(EDataName.Target));

        if (constructable != null && ColonistUtility.ReachedDestination(agent, constructablePosition))
        {
            var data = GetData(EDataName.InventoryIndex);
            if (data == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            int itemIndex = (int)data;
            constructable.AddItem(colonistData.inventory.TakeItemOut(itemIndex));
            ClearData(EDataName.InventoryItem);
            ClearData(EDataName.Target);
            ClearData(EDataName.Cost);

            if (constructable.CheckIfCostsFulfilled())
            {
                constructable.ConstructBuilding();
                ClearData(EDataName.Constructable);
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}