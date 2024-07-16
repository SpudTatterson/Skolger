using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTree.Tree;

public class ColonistBT : Tree
{
    [SerializeField] private WanderSettingsSO wanderSettings;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected override Node SetupTree()
    {
        Node wanderTask = CreateWanderTask();
        Node haulTask = CreateHaulTask();

        Node root = new Selector(new List<Node>
        {
            wanderTask,
            haulTask
        });

        return root;
    }


    private Node CreateWanderTask()
    {
        return new TaskWander(agent, wanderSettings)
        {
            priority = wanderSettings.priority
        };
    }

    private Node CreateHaulTask()
    {
        return new Sequence(new List<Node>
        {
            new CheckHaulableInReach(agent),
            new TaskGoToTarget(agent),
            new CheckInPickupRange()
        })
        {
            priority = 1
        };
    }
}
