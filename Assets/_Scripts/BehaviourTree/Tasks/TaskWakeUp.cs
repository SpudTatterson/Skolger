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
            
            if (BedManager.tempBeds.ContainsKey(colonistData))
            {
                BedManager.tempBeds[colonistData].Deconstruct();
            }
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