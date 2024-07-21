using System.Runtime.InteropServices;
using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TaskWander : Node
{
    private ColonistSettingsSO colonistSettings;

    private float currentWaitTime;

    private NavMeshAgent agent;

    public TaskWander(NavMeshAgent agent, ColonistSettingsSO colonistSettings)
    {
        this.agent = agent;
        this.colonistSettings = colonistSettings;
        currentWaitTime = Random.Range(0f, colonistSettings.maxWaitTime);
    }

    public override NodeState Evaluate()
    {
        if (agent.velocity == Vector3.zero)
        {
            currentWaitTime += Time.deltaTime;
        }

        if (currentWaitTime >= colonistSettings.maxWaitTime)
        {
            SetRandomDestination();
            currentWaitTime = Random.Range(0f, colonistSettings.maxWaitTime / 2);
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * colonistSettings.waypointRange;
        randomDirection += agent.transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, colonistSettings.waypointRange, 1))
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