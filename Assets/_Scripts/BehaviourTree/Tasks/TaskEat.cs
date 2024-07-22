using BehaviorTree;
using UnityEngine;

public class TaskEat : Node
{
    ColonistData colonistData;

    public TaskEat(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Ate something");

        state = NodeState.RUNNING;
        return state;
    }
}