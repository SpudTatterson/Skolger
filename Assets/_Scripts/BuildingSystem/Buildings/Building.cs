using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building")]
public class Building : ScriptableObject
{
  public int length = 1;
  public int width = 1;
  public List<MaterialCost> costs = new List<MaterialCost>();   
}