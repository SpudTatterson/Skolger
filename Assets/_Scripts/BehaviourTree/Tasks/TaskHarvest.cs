using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;

public class TaskHarvest : Node
{
    public override NodeState Evaluate()
    {
        IHarvestable harvestable = (IHarvestable)GetData("Harvestable");

        if (harvestable != null && !harvestable.IsBeingHarvested())
        {
            harvestable.StartHarvesting();
            Debug.Log("Started harvesting");

            state = NodeState.RUNNING;
            return state;
        }

        if (harvestable.IsBeingHarvested())
        {
            Debug.Log("Being harvested");

            
            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}