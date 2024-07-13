using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskPatrol : Node
{
    private NavMeshAgent agent;
    private Vector3 target;
    private float waypointRange;

    private float currentWaitTime;
    private float maxWaitTime;

    public TaskPatrol(NavMeshAgent agent, float maxWaitTime, float waypointRange)
    {
        this.agent = agent;
        target = agent.transform.position;

        this.maxWaitTime = maxWaitTime;
        currentWaitTime = Random.Range(0f, maxWaitTime);

        this.waypointRange = waypointRange;
    }

    public override NodeState Evaluate()
    {
        var distanceToTarget = Vector3.Distance(agent.transform.position, target);

        if (distanceToTarget <= 0.1f)
        {
            currentWaitTime += Time.deltaTime;
        }

        if (currentWaitTime >= maxWaitTime)
        {
            SetRandomDestination();
            currentWaitTime = Random.Range(0f, maxWaitTime / 2);
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * waypointRange;
        randomDirection += agent.transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, waypointRange, 1))
        {
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
            target = finalPosition;
        }
        else
        {
            SetRandomDestination();
        }
    }
}