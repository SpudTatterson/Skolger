using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableData : ScriptableObject
{
    public int xSize = 1;
    public int ySize = 1;
    public bool takesFullCell;
    public bool walkable;
    public List<ItemCost> costs = new List<ItemCost>();
}
