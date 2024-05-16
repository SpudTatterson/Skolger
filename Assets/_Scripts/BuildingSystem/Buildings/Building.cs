using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building/Building")]
public class Building : Placeable
{
  public List<MaterialCost> costs = new List<MaterialCost>();
  public GameObject building;   
}
