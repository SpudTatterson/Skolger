using BehaviorTree;

class CheckForCorrectItem : Node
{
    ColonistData colonistData;

    public CheckForCorrectItem(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        ItemCost cost = (ItemCost)GetData(EDataName.Cost);
        if(colonistData.HasItem(cost.item, cost.cost, out _))
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}