using BehaviorTree;

class CheckIfNotTired : Node
{
    private ColonistData colonistData;

    public CheckIfNotTired(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        if(colonistData.restManger.IsWellRested())
        {
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