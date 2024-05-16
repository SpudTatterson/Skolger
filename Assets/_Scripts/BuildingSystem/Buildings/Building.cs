using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building/Building")]
public class Building : Placeable
{
  public List<ItemCost> costs = new List<ItemCost>();
  public GameObject building;   
}
