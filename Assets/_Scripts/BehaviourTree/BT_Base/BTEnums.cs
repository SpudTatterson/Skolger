using UnityEngine;

public enum EDataName
{
    Target,
    FollowTarget,
    Constructable,
    InventoryItem,
    Cost,
    FoodData,
    Stockpile,
    Harvestable,
    Cell,
    InventoryIndex
}

public enum ETaskDescription
{
    Eating,
    GettingItemToHaul,
    HaulingToStockpile,
    Constructing,
    ItemsToConstruct,
    Wandering,
    Harvesting
}

public enum BrainState
{
    Unrestricted,
    Work,
    Rest,
    Recreational,
    Drafted,
    Breakdown,
    Sleeping
}