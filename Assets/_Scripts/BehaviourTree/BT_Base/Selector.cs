using System.Collections.Generic;
using System.Linq;

namespace BehaviorTree
{
    public class Selector : Node
    {
        public Selector() : base() {}
        public Selector(List<Node> children) : base(children) {}

        public override NodeState Evaluate()
        {
            foreach (Node node in children.OrderByDescending(c => c.priority))
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }                
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}