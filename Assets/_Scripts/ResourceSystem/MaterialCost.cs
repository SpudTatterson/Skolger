using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCost 
{
    public MaterialType type;
    public int cost;

    public MaterialCost(MaterialType type, int cost)
    {
        this.type = type;
        this.cost = cost;
    } 

    public override string ToString()
    {
        return string.Format("Type: ", type, "Cost: ", cost.ToString());
    }
}
