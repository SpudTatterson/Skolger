using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTree.Tree;

[RequireComponent(typeof(NavMeshAgent))]
public class AxeManBT : Tree
{
    [Header("Task Prioreties")]
    [SerializeField] private int patrolTask;
    [SerializeField] private int huntTask;

    [Header("Tasks Settings")]
    [Header("Patrol Settings")]
    [SerializeField] private float maxWaitTime = 5f;
    [SerializeField] private float waypointRange = 10f;

    [Header("Hunt Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask targetLayerMask;

    private NavMeshAgent agent;

    protected override Node SetupTree()
    {
        Node checkEnemySequence = CreateCheckEnemySequence();
        Node patrolTask = CreatePatrolTask();

        Node root = new Selector(new List<Node> 
        {
            checkEnemySequence,
            patrolTask
        });

        return root;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private Node CreateCheckEnemySequence()
    {
        return new Sequence(new List<Node>
        {
            new CheckEnemyInRange(agent.transform, detectionRadius, targetLayerMask),
            new TaskGoToTarget(agent)
        })
        {
            priority = huntTask
        };
    }

    private Node CreatePatrolTask()
    {
        return new TaskPatrol(agent, maxWaitTime, waypointRange)
        {
            priority = patrolTask
        };
    }
}