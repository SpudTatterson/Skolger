using BehaviorTree;
using UnityEngine;

public class CheckForEatable : Node
{
    public override NodeState Evaluate()
    {
        //arab time
        MonoBehaviour eatable = MonoBehaviour.FindObjectOfType<FoodTest>();

        if(eatable != null)
        {
            Debug.Log("Found Eatable");
            parent.parent.SetData("Food", eatable);
            parent.parent.SetData("Target", eatable);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}