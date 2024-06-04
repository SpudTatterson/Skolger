using CrashKonijn.Goap.Behaviours;
using UnityEngine;

[RequireComponent(typeof(AgentBehaviour))]
public class AxeManBrain : MonoBehaviour
{
    private AgentBehaviour AgentBehaviour;

    private void Awake()
    {
        AgentBehaviour = GetComponent<AgentBehaviour>();
    }

    private void Start()
    {
        AgentBehaviour.SetGoal<WanderGoal>(false);
    }
}