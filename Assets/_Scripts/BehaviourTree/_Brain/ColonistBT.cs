using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTree.Tree;
using Random = UnityEngine.Random;

public class ColonistBT : Tree
{
    [SerializeField] private ColonistSettingsSO colonistSettings;

    private NavMeshAgent agent;
    private ColonistData colonistData;
    private Dictionary<TaskKey, string> taskDescriptions;

    //states cache
    private Node workStateRoot;
    private Node unrestrictedStateRoot;
    private Node restingStateRoot;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        colonistData = GetComponent<ColonistData>();
        taskDescriptions = TaskDescriptions();
    }

    protected override void Update()
    {
        base.Update();
        UpdateBrainState();  // Check and update brain state each frame
    }

    private void UpdateBrainState()
    {
        switch (colonistData.brainState)
        {
            case BrainState.Work:
                root = workStateRoot;
                break;
            case BrainState.Unrestricted:
                root = unrestrictedStateRoot;
                break;
            case BrainState.Rest:
                root = restingStateRoot;
                break;

        }
    }


    #region Behaviour Tree Setup

    // This will be called once at the start to setup all root nodes.
    protected override Node SetupTree()
    {
        // Setup different root nodes based on states
        workStateRoot = SetupWorkState();
        unrestrictedStateRoot = SetupUnrestrictedState();
        restingStateRoot = SetupRestingState();

        // Return the default state to start with (e.g., Wandering)
        return unrestrictedStateRoot;
    }

    private Node SetupWorkState()
    {
        return new Selector(new List<Node>
        {
            CreateTaskWander(),

            CreateTaskHaul(),
            CreateTaskConstruct(),
            CreateTaskHarvest()
        });
    }

    private Node SetupUnrestrictedState()
    {
        return new Selector(new List<Node>
        {
            CreateTaskEat(),
            CreateTaskWander(),

            CreateTaskHaul(),
            CreateTaskConstruct(),
            CreateTaskHarvest()
        });
    }

    private Node SetupRestingState()
    {
        return new Selector(new List<Node>
        {
            CreateTaskEat(),
            CreateTaskWander()
        });
    }
    }
    #endregion

    #region Eating Task
    private Node CreateTaskEat()
    {
        return new Sequence(new List<Node>
        {
            new CheckIfHungry(colonistData),
            new CheckForEatable(),
            new TaskDropInventoryItem(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskKey.Eating]),
            new TaskEat(agent, colonistData)
        })
        {
            priority = colonistSettings.taskEat,
        };
    }
    #endregion

    #region Wandering Task
    private Node CreateTaskWander()
    {
        return new TaskWander(agent, colonistSettings, colonistData, taskDescriptions[TaskKey.Wandering])
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
            new CheckForStockpile(agent,colonistData),
            new CheckIsAbleToHaul(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskKey.GettingItemToHaul]),
            new TaskPickUpItem(agent, colonistData)
        })
        {
            priority = 0
        };

        Node haulToStockpileSequence = new Sequence(new List<Node>
        {
            new CheckItemInInventory(colonistData),
            new CheckForStockpile(agent,colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskKey.HaulingToStockpile]),
            new TaskPutInStockpile(agent, colonistData)
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
    private Node CreateTaskConstruct()
    {
        List<DataName> requiredKeys = new List<DataName>
        {
            DataName.Constructable,
            DataName.InventoryItem,
            DataName.Cost
        };

        Node getItemsFromStockpile = new Sequence(new List<Node>
        {
            new CheckForConstructable(colonistData),
            new CheckForConstructableCost(),
            new CheckHasItem(),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskKey.ItemsToConstruct]),
            new TaskTakeItemFromStockpile(agent, colonistData),
        })
        {
            priority = 0
        };

        Node placeItemsInConstruction = new Sequence(new List<Node>
        {
            new CheckForCorrectData(requiredKeys),
            new CheckForCorrectItem(colonistData),
            new CheckItemInInventory(colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskKey.Constructing]),
            new TaskPutItemInConstructable(agent, colonistData)
        })
        {
            priority = 1
        };

        return new Selector(new List<Node>
        {
            getItemsFromStockpile,
            placeItemsInConstruction
        })
        {
            priority = colonistSettings.taskConstruction
        };
    }
    #endregion

    #region Harvest Task
    private Node CreateTaskHarvest()
    {
        return new Sequence(new List<Node>
        {
            new CheckForHarvestable(),
            new TaskDropInventoryItem(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskKey.Harvesting]),
            new TaskHarvest(agent)
        })
        {
            priority = colonistSettings.taskHarvest
        };
    }
    #endregion

    #region Task Descriptions
    private Dictionary<TaskKey, string> TaskDescriptions()
    {
        Dictionary<TaskKey, string> taskDescriptions = new Dictionary<TaskKey, string>
        {
            {TaskKey.Eating, "Going to eat"},
            {TaskKey.Wandering, "Wandering"},
            {TaskKey.GettingItemToHaul, "Finding items to haul"},
            {TaskKey.HaulingToStockpile, "Hauling items to stockpile"},
            {TaskKey.ItemsToConstruct, "Going to get items for construction"},
            {TaskKey.Constructing, "Going to construct"},
            {TaskKey.Harvesting, "Going to harvest"},
        };

        return taskDescriptions;
    }
    #endregion
}