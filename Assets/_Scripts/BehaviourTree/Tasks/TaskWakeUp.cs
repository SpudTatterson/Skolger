using BehaviorTree;

class TaskWakeUp : Node
{
    private ColonistData colonistData;

    public TaskWakeUp(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        if (colonistData.restManger.sleeping)
        {
            colonistData.restManger.WakeUp();
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