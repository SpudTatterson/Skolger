using System.Runtime.InteropServices;
using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TaskWander : Node
{
    private WanderSettingsSO wanderSettings;

    private float currentWaitTime;

    private NavMeshAgent agent;

    public TaskWander(NavMeshAgent agent, WanderSettingsSO wanderSettings)
    {
        this.agent = agent;
        this.wanderSettings = wanderSettings;
        currentWaitTime = Random.Range(0f, wanderSettings.maxWaitTime);
    }

    public override NodeState Evaluate()
    {
        if (agent.velocity == Vector3.zero)
        {
            currentWaitTime += Time.deltaTime;
        }

        if (currentWaitTime >= wanderSettings.maxWaitTime)
        {
            SetRandomDestination();
            currentWaitTime = Random.Range(0f, wanderSettings.maxWaitTime / 2);
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderSettings.waypointRange;
        randomDirection += agent.transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderSettings.waypointRange, 1))
        {
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
        }
        else
        {
            SetRandomDestination();
        }
    }
}