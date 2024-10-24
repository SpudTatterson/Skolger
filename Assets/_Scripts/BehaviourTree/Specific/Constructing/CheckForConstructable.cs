using BehaviorTree;
using UnityEngine;

public class CheckForConstructable : Node
{
    ColonistData colonistData;
    public CheckForConstructable(ColonistData colonistData)
    {
        this.colonistData = colonistData;
    }
    public override NodeState Evaluate()
    {
        var hasConstructable = GetData(EDataName.Constructable);
        var hasInventoryItem = GetData(EDataName.InventoryItem);

        if (hasInventoryItem != null || !colonistData.inventory.IsEmpty())
        {
            state = NodeState.RUNNING;
            return state;
        }

        if (hasConstructable != null)
        {
            state = NodeState.SUCCESS;
            return state; 
        }

        var constructable = TaskManager.Instance.PullConstructableFromQueue();

        if (constructable != null)
        {
            Vector3 position = constructable.GetPosition().position;
            if (constructable is ConstructionSiteObject buildingObject && buildingObject.buildingData is FloorTile)
            {
                int heightModifier = Mathf.FloorToInt(position.y / GridManager.Instance.worldSettings.cellHeight);
                heightModifier = Mathf.RoundToInt(Mathf.Clamp(heightModifier - 1, 0, Mathf.Infinity));
                if (heightModifier == 0 && Mathf.RoundToInt(position.y) != 0) heightModifier++;
                position.y -= heightModifier * GridManager.Instance.worldSettings.cellHeight;
            }
            if (colonistData.agent.CanReachPoint(position) && InventoryManager.Instance.HasItems(constructable.GetAllCosts()))
            {
                parent.parent.SetData(EDataName.Constructable, constructable);
                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                // mark constructable as unreachable and add to issue tracker
                TaskManager.Instance.AddToConstructionQueue(constructable);
                state = NodeState.FAILURE;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}