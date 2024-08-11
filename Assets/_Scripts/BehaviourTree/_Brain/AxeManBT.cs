using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTree.Tree;

[RequireComponent(typeof(NavMeshAgent))]
public class AxeManBT : Tree
{
    [Header("Task Prioreties")]
    [SerializeField] private int wanderTask;
    [SerializeField] private int huntTask;

    [SerializeField] private ColonistSettingsSO colonistSettings;

    [Header("Hunt Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask targetLayerMask;

    private NavMeshAgent agent;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected override Node SetupTree()
    {
        Node huntTask = CreateHuntTask();

        Node root = new Selector(new List<Node> 
        {
            huntTask,
        });

        return root;
    }

    private Node CreateHuntTask()
    {
        return new Sequence(new List<Node>
        {
            new CheckEnemyInRange(agent.transform, detectionRadius, targetLayerMask),
            new TaskFollowTarget(agent)
        })
        {
            priority = huntTask
        };
    }
}