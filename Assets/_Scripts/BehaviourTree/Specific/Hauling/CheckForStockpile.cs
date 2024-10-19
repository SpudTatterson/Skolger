using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CheckForStockpile : Node
{
    private NavMeshAgent agent;
    ColonistData colonistData;

    public CheckForStockpile(NavMeshAgent agent, ColonistData colonistData)
    {
        this.agent = agent;
        this.colonistData = colonistData;
    }

    public override NodeState Evaluate()
    {
        Stockpile stockpile = InventoryManager.Instance.GetStockpileWithEmptySpace(out Cell cell);

        if (cell == null)
        {
            object hasStockpile = GetData(EDataName.Stockpile);

            if (!colonistData.inventory.IsEmpty())
            {
                int itemIndex = (int)GetData(EDataName.InventoryIndex);
                colonistData.inventory.TakeItemOut(itemIndex).DropItem(agent.transform.position);
                agent.ResetPath();

                ClearData(EDataName.InventoryItem);
            }

            if (hasStockpile != null)
            {
                agent.ResetPath();

                ClearData(EDataName.Stockpile);
                ClearData(EDataName.Cell);
            }

            state = NodeState.FAILURE;
            return state;
        }

        var target = GetData(EDataName.Target);
        if (target == null)
        {
            if (agent.CanReachPoint(ColonistUtility.ConvertToVector3(cell)))
            {
                parent.parent.SetData(EDataName.Target, cell);
                parent.parent.SetData(EDataName.Cell, cell);
                parent.parent.SetData(EDataName.Stockpile, stockpile);

                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                // add to issue tracker

                // ClearData(EDataName.Target);
                // ClearData(EDataName.Stockpile);
                // ClearData(EDataName.Cell);
                InventoryManager.Instance.stockpiles.Remove(stockpile);
                InventoryManager.Instance.stockpiles.Add(stockpile);
                state = NodeState.FAILURE;
                return state;

            }
        }
        else
        {
            state = NodeState.SUCCESS;
            return state;
        }

    }
}