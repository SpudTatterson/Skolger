using System;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using Tree = BehaviorTree.Tree;

public class ColonistBT : Tree
{
    [SerializeField] ColonistSettingsSO colonistSettings;
    [Space]
    [BoxGroup("Initial set, player access locked")]
    [SerializeField] private int taskEat;
    [BoxGroup("Initial set, player access locked")]
    [SerializeField] private int taskWander;
    [BoxGroup("Initial set, player access locked")]
    [SerializeField] private int taskSleep;
    [Space]
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

    private void LateUpdate()
    {
        if (rearrangeTree)
        {
            RearrangeTree();
            rearrangeTree = false;
        }
    }
        
    #region Behaviour Tree Setup

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            SetupWorkState(),
            SetupUnrestrictedState(),
            SetupRestingState(),
            SetupSleepState()
        });

        return root;
    }
    #endregion

    #region Brain State Setup
    private Node SetupSleepState()
    {
        return new Sequence(new List<Node>
        {
            new CheckForBrainState(colonistData, EBrainState.Sleep),              // Checks if the brain is in the correct state
            CreateTaskWakeUp()
        });
    }


    private Node SetupWorkState()
    {
        return new Sequence(new List<Node>
        {
            new CheckForBrainState(colonistData, EBrainState.Work),                 // Checks if the brain is in the correct state
            new Selector(new List<Node>
            {
                new TaskWakeUp(colonistData),
                CreateTaskWander(),
                CreateTaskHaul(),
                CreateTaskConstruct(),
                CreateTaskHarvest()
            })
        });
    }

    private Node SetupUnrestrictedState()
    {
        return new Sequence(new List<Node>
        {
            new CheckForBrainState(colonistData, EBrainState.Unrestricted),          // Checks if the brain is in the correct state
            new Selector(new List<Node>
            {
                new TaskWakeUp(colonistData),
                CreateTaskEat(),
                CreateTaskWander(),
                CreateTaskHaul(),
                CreateTaskConstruct(),
                CreateTaskHarvest()
            })
        });
    }

    private Node SetupRestingState()
    {        
        return new Sequence(new List<Node>
        {
            new CheckForBrainState(colonistData, EBrainState.Rest),                 // Checks if the brain is in the correct state
            new Selector(new List<Node>
            {
                CreateTaskSleep(),
                CreateTaskEat(),
                CreateTaskWander()
            })
        });

    }

    private Node CreateTaskWakeUp()
    {
        return new Sequence(new List<Node>
        {
            new CheckIfNotTired(colonistData),
            new TaskWakeUp(colonistData),
        })
        {
            priority = taskSleep,
        };
    }
    #endregion

    // private Node GetBreakDownNode()
    // {
    //     switch (colonistData.moodManager.breakDownType)
    //     {
    //         case BreakDownType.Wander:
    //             return CreateTaskWander();
    //         case BreakDownType.EatingFrenzy:
    //             return CreateTaskEat();

    //         default:
    //             return unrestrictedStateRoot;
    //     }
    // }

    #region Sleeping Task
    private Node CreateTaskSleep()
    {
        return new Sequence(new List<Node>
        {
            new CheckIfTired(colonistData),
            new CheckForBed(colonistData),
            new TaskGoToTarget(agent),
            new TaskGoToSleep(colonistData),
        })
        {
            priority = taskSleep
        };
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
            priority = taskEat
        };
    }
    #endregion

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
        Node taskPutInStockpile = new TaskPutInStockpile(agent, colonistData);

        Node pickUpItemSequence = new Sequence(new List<Node>
        {
            new CheckForStockpile(agent,colonistData),
            new CheckIsAbleToHaul(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.GettingItemToHaul]),
            new TaskPickUpItem(agent, colonistData)
        })
        {
            priority = 0,
        };

        Node haulToStockpileSequence = new Sequence(new List<Node>
        {
            new CheckItemInInventory(colonistData),
            new CheckForStockpile(agent,colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.HaulingToStockpile]),
            taskPutInStockpile,
            new RearrangeTree(taskPutInStockpile, this)
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
        Node taskPutItemInConstructable = new TaskPutItemInConstructable(agent, colonistData);

        List<Enum> requiredKeys = new List<Enum>
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
            taskPutItemInConstructable,
            new RearrangeTree(taskPutItemInConstructable, this)
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
        Node harvestTask = new TaskHarvest(agent);

        return new Sequence(new List<Node>
        {
            new CheckForHarvestable(agent),
            new TaskDropInventoryItem(agent, colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[ETaskDescription.Harvesting]),
            harvestTask,
            new RearrangeTree(harvestTask, this)
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