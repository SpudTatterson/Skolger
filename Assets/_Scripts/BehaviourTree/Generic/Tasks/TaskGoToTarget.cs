using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private NavMeshAgent agent;
    private string taskDescription;
    private ColonistData colonistData;

    public TaskGoToTarget(NavMeshAgent agent)
    {
        this.agent = agent;
    }
    public TaskGoToTarget(NavMeshAgent agent, ColonistData colonistData, string taskDescription)
    {
        this.agent = agent;
        this.taskDescription = taskDescription;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        object target = GetData(EDataName.Target);

        if (target is MonoBehaviour monoBehaviour)
        {
            target = monoBehaviour.transform.position;
        }
        else if (target is Cell cell)
        {
            target = cell.position;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }

        agent.SetDestination((Vector3)target);
        if (colonistData != null)
        {
            colonistData.ChangeActivity(taskDescription);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
