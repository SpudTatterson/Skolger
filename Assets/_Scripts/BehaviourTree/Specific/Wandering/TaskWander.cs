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
    private string taskDescription;
    private ColonistData colonistData;
    private ColonistBT colonistBT;

    public TaskWander(NavMeshAgent agent, ColonistSettingsSO colonistSettings, ColonistData colonistData, ColonistBT colonistBT, string taskDescription)
    {
        this.agent = agent;
        this.colonistSettings = colonistSettings;
        this.colonistData = colonistData;
        this.taskDescription = taskDescription;
        this.colonistBT = colonistBT;
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
        colonistData.ChangeActivity(taskDescription);

        if (colonistBT.rearrangeTree)
        {
            colonistBT.TriggerTreeSetup();
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