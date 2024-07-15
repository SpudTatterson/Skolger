using BehaviorTree;
using UnityEngine;

public class CheckEnemyInRange : Node
{
    private Transform transform;
    private float detectionRadius;
    private LayerMask targetLayerMask;

    public CheckEnemyInRange(Transform transform, float detectionRadius, LayerMask targetLayerMask)
    {
        this.transform = transform;

        this.detectionRadius = detectionRadius;

        this.targetLayerMask = targetLayerMask;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("Target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayerMask);

            if(colliders.Length > 0)
            {
                parent.parent.SetData("Target", colliders[0].transform);

                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}