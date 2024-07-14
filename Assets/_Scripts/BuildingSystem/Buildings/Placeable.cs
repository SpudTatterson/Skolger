using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlaceableData : ScriptableObject
{
    [ShowAssetPreview, BoxGroup("Main Info")] public Texture icon;
    [BoxGroup("Main Info")] public string placeableName;
    [ResizableTextArea, BoxGroup("Main Info")] public string description;
    [BoxGroup("Main Info")] public List<ItemCost> costs = new List<ItemCost>();

    [BoxGroup("Settings")] public int xSize = 1;
    [BoxGroup("Settings")] public int ySize = 1;
    [BoxGroup("Settings")] public bool takesFullCell;
    [BoxGroup("Settings")] public bool walkable;
    [BoxGroup("Settings")] public PlacementType placementType = PlacementType.Single;
}

public enum PlacementType
{
    Single,
    Line,
    Square
}