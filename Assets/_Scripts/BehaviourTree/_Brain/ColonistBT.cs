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
        Node Task_wander = CreateTaskWander();
        Node Task_hauling = CreateTaskHaul();

        Node collectFromStockpileTask = CreateCollectFromStockpile();
        Node buildTask = CreateConstructionTask();

        Node root = new Selector(new List<Node>
        {
            // Basic AI tasks that the player can not access in game
            // and the priorities are set from the start.
            Task_wander,
            // ----------------------------------------------------
            // AI tasks that the player will have access in game
            // the priorities can change in runtime.
            Task_hauling,
            // ----------------------------------------------------
        });

        return root;
    }

    #region Wandering Task
    private Node CreateTaskWander()
    {
        return new TaskWander(agent, colonistSettings)
        {
            priority = colonistSettings.taskWander
        };
    }
    #endregion

    #region Hauling Task
    private Node CreateTaskHaul()
    {
        Node pickUpItemSequence = new Sequence(new List<Node>
        {
            new CheckForStockpile(agent),
            new CheckIsAbleToHaul(agent),
            new TaskGoToTarget(agent),
            new TaskPickUpItem(agent)
        })
        {
            priority = 0
        };

        Node haulToStockpileSequence = new Sequence(new List<Node>
        {
            new CheckItemInInventory(),
            new CheckForStockpile(agent),
            new TaskGoToTarget(agent),
            new TaskPutInStockpile(agent)
        })
        {
            priority = 1
        };

        return new Selector(new List<Node>
        {
            pickUpItemSequence,
            haulToStockpileSequence
        })
        {
            priority = colonistSettings.taskHaul
        };
    }
    #endregion

    #region Construction Task
    private Node CreateCollectFromStockpile()
    {
        return new Sequence(new List<Node>
        {
            new CheckForConstructable(),
            new CheckForConstructableCost(),
            new CheckHasItem(),
            new TaskGoToTarget(agent),
            new TaskTakeItemFromStockpile(agent),
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
    #endregion
}