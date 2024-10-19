using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskGoToSleep : Node
{
    ColonistData colonistData;
    NavMeshAgent agent;
    public TaskGoToSleep(ColonistData colonistData)
    {
        this.colonistData = colonistData;
        this.agent = colonistData.agent;
    }
    public override NodeState Evaluate()
    {
        object bedData = GetData(EDataName.Target);
        Vector3 bedPosition = ColonistUtility.ConvertToVector3(bedData);

        if (ColonistUtility.ReachedDestination(agent, bedPosition))
        {
            colonistData.restManger.Sleep();
            ClearData(EDataName.Target);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }
    }
}