using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckHasConstructableItem : Node
{
    NavMeshAgent agent;
    public CheckHasConstructableItem(NavMeshAgent agent)
    {
        this.agent = agent;
    }
    public override NodeState Evaluate()
    {
        ItemCost itemCost = (ItemCost)GetData(EDataName.Cost);

        if (itemCost != null)
        {
            if (InventoryManager.Instance.HasItem(itemCost))
            {
                Cell itemPosition = InventoryManager.Instance.GetItemLocation(itemCost.item, itemCost.cost, out Stockpile stockpile);

                if (agent.CanReachPoint(itemPosition.position))
                {
                    parent.parent.SetData(EDataName.Target, itemPosition);
                    parent.parent.SetData(EDataName.Stockpile, stockpile);

                    state = NodeState.SUCCESS;
                    return state;
                }
                else
                {
                    // add to issue tracker
                    InventoryManager.Instance.stockpiles.Remove(stockpile);
                    InventoryManager.Instance.stockpiles.Add(stockpile);
                    state = NodeState.FAILURE;
                    return state;
                }
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}