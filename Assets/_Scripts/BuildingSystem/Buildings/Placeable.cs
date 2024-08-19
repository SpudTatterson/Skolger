using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlaceableData : ScriptableObject
{
    [PreviewField, BoxGroup("Main Info")] public Texture icon;
    [BoxGroup("Main Info")] public string placeableName;
    [Multiline, BoxGroup("Main Info")] public string description;
    [BoxGroup("Main Info")] public List<ItemCost> costs = new List<ItemCost>();

    [BoxGroup("Settings")] public int xSize = 1;
    [BoxGroup("Settings")] public int ySize = 1;
    [BoxGroup("Settings")] public bool usesCell;
    [BoxGroup("Settings"), Tooltip("Does this placeable visually take the whole cell")] public bool takesFullCell;
    [BoxGroup("Settings")] public bool walkable;
    [BoxGroup("Settings"), SerializeField] PlacementType placementType;
    IPlacementStrategy placementStrategy;

    public IPlacementStrategy PlacementStrategy
    {
        get
        {
            if (placementStrategy == null)
            {
                placementStrategy = GetPlacementStrategy();
            }
            return placementStrategy;
        }
    }

    IPlacementStrategy GetPlacementStrategy()
    {
        switch (placementType)
        {
            case PlacementType.Line:
                return new LinePlacementStrategy();
                
            case PlacementType.Single:
                return new SinglePlacementStrategy();
                
            case PlacementType.Square:
                return new SquarePlacementStrategy();
                
        }
        throw new NullReferenceException("No placement strategy found for selection type");
    }
}

public enum PlacementType
{
    Single,
    Line,
    Square
}