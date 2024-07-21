using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTree.Tree;

public class ColonistBT : Tree
{
    [SerializeField] private ColonistSettingsSO colonistSettings;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected override Node SetupTree()
    {
        Node wanderTask = CreateWanderTask();
        Node pickUpItemTask = CreatePickUpItemTask();
        Node haulToStockpileTask = CreateHaulToStockpile();
        Node collectFromStockpileTask = CreateCollectFromStockpile();
        Node buildTask = CreateConstructionTask();

        Node root = new Selector(new List<Node>
        {
            // Basic AI tasks that the player can not access in game
            // and the priorities are set from the start.

            wanderTask,
            haulToStockpileTask,

            // ----------------------------------------------------
            // AI tasks that the player will have access in game
            // the priorities can change in runtime.
            
            collectFromStockpileTask,
            pickUpItemTask,
            buildTask
        });

        return root;
    }


    private Node CreateWanderTask()
    {
        return new TaskWander(agent, colonistSettings)
        {
            priority = colonistSettings.priorityWander
        };
    }

    private Node CreatePickUpItemTask()
    {
        return new Sequence(new List<Node>
        {
            new CheckForStockpile(agent),
            new CheckIsAbleToHaul(agent),
            new TaskGoToTarget(agent),
            new TaskPickUpItem(agent)
        })
        {
            priority = colonistSettings.priorityPickUpItem
        };
    }

    private Node CreateHaulToStockpile()
    {
        return new Sequence(new List<Node>
        {
            new CheckItemInInventory(),
            new CheckForStockpile(agent),
            new TaskGoToTarget(agent),
            new TaskPutInStockpile(agent)
        })
        {
            priority = colonistSettings.priorityHaulToStockpile
        };
    }

    private Node CreateCollectFromStockpile()
    {
        return new Sequence(new List<Node>
        {
            new CheckForConstructable(),
            new CheckForConstructableCost(),
            new CheckHasItem(),
            new TaskGoToTarget(agent),
            new TaskTakeItemFromStockpile(),
        })
        {
            priority = 1000
        };
    }

    private Node CreateConstructionTask()
    {
        return new Sequence(new List<Node>
        {
            new CheckItemInInventory(),
            new TaskGoToTarget(agent),
            new TaskPutItemInConstructable(agent)
        })
        {
            priority = 1001
        };
    }
}
