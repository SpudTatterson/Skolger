using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private NavMeshAgent agent;

    public TaskGoToTarget(NavMeshAgent agent) 
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        object target = GetData("Target");
        
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

        state = NodeState.RUNNING;
        return state;
    }
}
