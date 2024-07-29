using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using Tree = BehaviorTree.Tree;

public class ColonistBT : Tree
{
    [SerializeField] private ColonistSettingsSO colonistSettings;

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

    #region Behaviour Tree Setup
    protected override Node SetupTree()
    {
        Node Task_Wander = CreateTaskWander();
        Node Task_Eat = CreateTaskEat();

        Node Task_Hauling = CreateTaskHaul();
        Node Task_Constructing = CreateTaskConstruct();
        Node Task_Harvesting = CreateTaskHarvest();

        Node root = new Selector(new List<Node>
        {
            // Basic AI tasks that the player can not access in game
            // and the priorities are set from the start.
            Task_Eat,
            Task_Wander,
            // ----------------------------------------------------
            // AI tasks that the player will have access in game
            // the priorities can change in runtime.
            Task_Hauling,
            Task_Constructing,
            Task_Harvesting
            // ----------------------------------------------------
        });

        return root;
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
            new CheckHasConstructableItem(),
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
            new CheckForHarvestable(),
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