using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTree.Tree;

public class ColonistBT : Tree
{
    [SerializeField] private ColonistSettingsSO colonistSettings;

    private NavMeshAgent agent;
    private ColonistData colonistData;
    private Dictionary<TaskDescription, string> taskDescriptions;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        colonistData = GetComponent<ColonistData>();
        taskDescriptions = TaskDescriptions();
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
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskDescription.Eating]),
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
        return new TaskWander(agent, colonistSettings, colonistData, taskDescriptions[TaskDescription.Wandering])
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
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskDescription.GettingItemToHaul]),
            new TaskPickUpItem(agent, colonistData)
        })
        {
            priority = 0
        };

        Node haulToStockpileSequence = new Sequence(new List<Node>
        {
            new CheckItemInInventory(colonistData),
            new CheckForStockpile(agent,colonistData),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskDescription.HaulingToStockpile]),
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
            new CheckHasConstructableItem(),
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskDescription.ItemsToConstruct]),
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
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskDescription.Constructing]),
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
            new TaskGoToTarget(agent, colonistData, taskDescriptions[TaskDescription.Harvesting]),
            new TaskHarvest(agent)
        })
        {
            priority = colonistSettings.taskHarvest
        };
    }
    #endregion

    #region Task Descriptions
    private Dictionary<TaskDescription, string> TaskDescriptions()
    {
        Dictionary<TaskDescription, string> taskDescriptions = new Dictionary<TaskDescription, string>
        {
            {TaskDescription.Eating, "Going to eat"},
            {TaskDescription.Wandering, "Wandering"},
            {TaskDescription.GettingItemToHaul, "Finding items to haul"},
            {TaskDescription.HaulingToStockpile, "Hauling items to stockpile"},
            {TaskDescription.ItemsToConstruct, "Going to get items for construction"},
            {TaskDescription.Constructing, "Going to construct"},
            {TaskDescription.Harvesting, "Going to harvest"},
        };

        return taskDescriptions;
    }
    #endregion
}