using BehaviorTree;

public class RearrangeTree : Node
{
    Node lastTaskNode;
    ColonistBT brain;

    public RearrangeTree(Node lastTaskNode, ColonistBT brain)
    {
        this.lastTaskNode = lastTaskNode;
        this.brain = brain;
    }

    public override NodeState Evaluate()
    {
        if(lastTaskNode.state == NodeState.SUCCESS)
        {
            if(brain.rearrangeTree)
            {
                brain.TriggerTreeSetup();
            }
            state = NodeState.SUCCESS;
        }
        else if(lastTaskNode.state == NodeState.RUNNING)
        {
            state = NodeState.RUNNING;
        }

        return state;
    }
}