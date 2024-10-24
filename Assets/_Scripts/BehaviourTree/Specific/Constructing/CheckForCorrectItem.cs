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
        if (colonistData.inventory.HasItem(cost.item, cost.cost, out int? InventoryIndex))
        {
            SetDataOnRoot(EDataName.InventoryIndex, InventoryIndex);
            state = NodeState.SUCCESS;
            return state;
        }
        IConstructable constructable = (IConstructable)GetData(EDataName.Constructable);
        if (constructable != null && constructable.beingConstructed)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}