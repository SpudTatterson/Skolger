using System.Runtime.CompilerServices;
using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMesh), typeof(AgentBehaviour))]
public class AgentMoveBehavior : MonoBehaviour
{
    private NavMeshAgent NavMeshAgent;
    private AgentBehaviour AgentBehaviour;
    private ITarget CurrentTarget;
    [SerializeField] private float MinMoveDistance = 0.25f;
    private Vector3 LastPosition;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        AgentBehaviour = GetComponent<AgentBehaviour>();
    }

    private void OnEnable()
    {
        AgentBehaviour.Events.OnTargetChanged += EventsOnTargetChanged;
        AgentBehaviour.Events.OnTargetOutOfRange += EventsOnTargetOutOfRange;
    }

    private void OnDisable()
    {
        AgentBehaviour.Events.OnTargetChanged -= EventsOnTargetChanged;
        AgentBehaviour.Events.OnTargetOutOfRange -= EventsOnTargetOutOfRange;
    }

    private void EventsOnTargetOutOfRange(ITarget target)
    {

    }

        private void EventsOnTargetChanged(ITarget target, bool inRange)
    {
        CurrentTarget = target;
        LastPosition = CurrentTarget.Position;
        NavMeshAgent.SetDestination(target.Position);
    }

    private void Update()
    {
        if (CurrentTarget == null)
        {
            return;
        }

        if(MinMoveDistance <= Vector3.Distance(CurrentTarget.Position, LastPosition))
        {
            LastPosition = CurrentTarget.Position;
            NavMeshAgent.SetDestination(CurrentTarget.Position);
        }
    }
}