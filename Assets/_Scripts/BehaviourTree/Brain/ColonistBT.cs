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
        Node pickUpItem = CreatePickUpItemTask();
        Node haulToStockpile = CreateHaulToStockpile();

        Node root = new Selector(new List<Node>
        {
            // Basic AI tasks that the player can not access ingame
            // and the priorities are set from the start.

            wanderTask,
            haulToStockpile,

            // ----------------------------------------------------
            // AI tasks that the player will have access ingame
            // the priorities can change in runtime.
            
            pickUpItem
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
            new CheckIsAbleToHaul(agent),
            new CheckForStockpile(),
            new TaskGoToTarget(agent),
            new TaskPickUpItem()
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
            new TaskGoToStockpile(agent)
        })
        {
            priority = colonistSettings.priorityHaulToStockpile
        };
    }
}
