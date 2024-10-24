using BehaviorTree;
using UnityEngine;

public class CheckItemInInventory : Node
{
    ColonistData colonistData;
    public CheckItemInInventory(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        IConstructable constructable = (IConstructable)GetData(EDataName.Constructable);
        if (constructable != null && constructable.beingConstructed)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        var inventoryItem = GetData(EDataName.InventoryItem);
        if (inventoryItem == null || colonistData.inventory.IsEmpty())
        {
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}