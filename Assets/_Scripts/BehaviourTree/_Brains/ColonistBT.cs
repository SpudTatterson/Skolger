using System;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using Tree = BehaviorTree.Tree;
using Random = UnityEngine.Random;

public class ColonistBT : Tree
{
    [SerializeField] ColonistSettingsSO colonistSettings;

    // cache
    Node workStateRoot;
    Node unrestrictedStateRoot;
    Node restingStateRoot;
    Node sleepStateRoot;
    public bool treeRearrangement;

    [BoxGroup("Initial set, player access locked")]
    [SerializeField] private int taskEat;
    [BoxGroup("Initial set, player access locked")]
    [SerializeField] private int taskWander;

    [BoxGroup("Editable at runtime, player access available")]
    [SerializeField] private int taskHaul;
    [BoxGroup("Editable at runtime, player access available")]
    [SerializeField] private int taskConstruct;
    [BoxGroup("Editable at runtime, player access available")]
    [SerializeField] private int taskHarvest;

    private NavMeshAgent agent;
    private ColonistData colonistData;
    private Dictionary<ETaskDescription, string> taskDescriptions;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        colonistData = GetComponent<ColonistData>();
        taskDescriptions = TaskDescriptions();
    }

    void LateUpdate()
    {
        if (treeRearrangement)
        {
            RearrangeTree();
            treeRearrangement = false;
        }
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
            case BrainState.Breakdown:
                root = GetBreakDownNode();
                break;
            case BrainState.Sleeping:
                root = sleepStateRoot;
                break;
            default: throw new NotImplementedException("Didn't find case for current brain state");
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
        sleepStateRoot = SetupSleepState();

        // Return the default state to start with (e.g., Wandering)
        return unrestrictedStateRoot;
    }

    private Node SetupSleepState()
    {
        return CreateTaskWakeUp();
    }

    private Node CreateTaskWakeUp()
    {
        return new Sequence(new List<Node>{
            new CheckIfNotTired(colonistData),
            new TaskWakeUp(colonistData),
        })
        {
            priority = colonistSettings.taskSleep,
        };
    }

    private Node SetupWorkState()
    {
        return new Selector(new List<Node>
        {
            new TaskWakeUp(colonistData),
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
            new TaskWakeUp(colonistData),
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
            CreateTaskSleep(),
            CreateTaskEat(),
            CreateTaskWander()
        });
    }

    private Node GetBreakDownNode()
    {
        switch (colonistData.moodManager.breakDownType)
        {
            case BreakDownType.Wander:
                return CreateTaskWander();
            case BreakDownType.EatingFrenzy:
                return CreateTaskEat();

            default:
                return unrestrictedStateRoot;
        }
    }

    #endregion

    #region Eating Task
    private Node CreateTaskEat()
    {
        return new Sequence(new List<Node>
        {
            new CheckIfHungry(colonistData),
            new CheckForEdible(),
            new TaskDropInventoryItem(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.Eating]),
            new TaskEat(agent, colonistData)
        })
        {
            priority = taskEat,
        };
    }
    #endregion

    Node CreateTaskSleep()
    {
        return new Sequence(new List<Node>{
            new CheckIfTired(colonistData),
            new CheckForBed(colonistData),
            new TaskGoToTarget(agent),
            new TaskGoToSleep(colonistData),
        })
        {
            priority = colonistSettings.taskSleep,
        };
    }

    #region Wandering Task
    private Node CreateTaskWander()
    {
        return new TaskWander(agent, colonistSettings, colonistData, taskDescriptions[ETaskDescription.Wandering])
        {
            priority = taskWander
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
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.GettingItemToHaul]),
            new TaskPickUpItem(agent, colonistData)
        })
        {
            priority = 0
        };

        Node haulToStockpileSequence = new Sequence(new List<Node>
        {
            new CheckItemInInventory(colonistData),
            new CheckForStockpile(agent,colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.HaulingToStockpile]),
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
            priority = taskHaul
        };
    }
    #endregion

    #region Construction Task
    private Node CreateTaskConstruct()
    {
        List<EDataName> requiredKeys = new List<EDataName>
        {
            EDataName.Constructable,
            EDataName.InventoryItem,
            EDataName.Cost
        };

        Node getItemsFromStockpile = new Sequence(new List<Node>
        {
            new CheckForConstructable(colonistData),
            new CheckForConstructableCost(),
            new CheckHasConstructableItem(agent),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.ItemsToConstruct]),
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
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.Constructing]),
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
            priority = taskConstruct
        };
    }
    #endregion

    #region Harvest Task
    private Node CreateTaskHarvest()
    {
        return new Sequence(new List<Node>
        {
            new CheckForHarvestable(agent),
            new TaskDropInventoryItem(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.Harvesting]),
            new TaskHarvest(agent)
        })
        {
            priority = taskHarvest
        };
    }
    #endregion

    #region Task Descriptions
    private Dictionary<ETaskDescription, string> TaskDescriptions()
    {
        Dictionary<ETaskDescription, string> taskDescriptions = new Dictionary<ETaskDescription, string>
        {
            {ETaskDescription.Eating, "Going to eat"},
            {ETaskDescription.Wandering, "Wandering"},
            {ETaskDescription.GettingItemToHaul, "Finding items to haul"},
            {ETaskDescription.HaulingToStockpile, "Hauling items to stockpile"},
            {ETaskDescription.ItemsToConstruct, "Going to get items for construction"},
            {ETaskDescription.Constructing, "Going to construct"},
            {ETaskDescription.Harvesting, "Going to harvest"},
        };

        return taskDescriptions;
    }
    #endregion
}