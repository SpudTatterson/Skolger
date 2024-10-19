using BehaviorTree;
using UnityEngine;

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
        if(colonistData.inventory.HasItem(cost.item, cost.cost, out int? InventoryIndex))
        {
            Debug.Log("ASGSWAEGFASDFAS");
            SetDataOnRoot(EDataName.InventoryIndex, InventoryIndex);
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}