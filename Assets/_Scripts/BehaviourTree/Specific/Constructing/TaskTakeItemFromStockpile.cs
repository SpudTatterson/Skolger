using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskTakeItemFromStockpile : Node
{
    NavMeshAgent agent;
    ColonistData colonistData;

    public TaskTakeItemFromStockpile(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        var cost = (ItemCost)GetData(EDataName.Cost);
        object cell = GetData(EDataName.Target);
        Vector3 cellPosition = ColonistUtility.ConvertToVector3(cell);

        if (cost != null && ColonistUtility.ReachedDestination(agent, cellPosition))
        {
            Stockpile stockpile = (Stockpile)GetData(EDataName.Stockpile);
            var item = InventoryManager.Instance.TakeItem(cost, stockpile);
            parent.parent.SetData(EDataName.InventoryItem, item);
            colonistData.inventory.PutItemIn(item);
            var constructable = (IConstructable)GetData(EDataName.Constructable);
            ClearData(EDataName.Target);

            Vector3 position = constructable.GetPosition().position;
            if (constructable is BuildingObject buildingObject && buildingObject.buildingData is FloorTile)
            {
                int heightModifier = Mathf.FloorToInt(position.y / GridManager.Instance.worldSettings.cellHeight);
                heightModifier = Mathf.RoundToInt(Mathf.Clamp(heightModifier - 1, 0, Mathf.Infinity));
                if (heightModifier == 0 && Mathf.RoundToInt(position.y) != 0) heightModifier++;
                position.y -= heightModifier * GridManager.Instance.worldSettings.cellHeight;
            }
            parent.parent.SetData(EDataName.Target, position);

            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.RUNNING;
        return state;
    }
}
