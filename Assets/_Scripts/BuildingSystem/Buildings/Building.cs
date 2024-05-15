using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building")]
public class Building : ScriptableObject
{
  public int xSize = 1;
  public int ySize = 1;
  public List<MaterialCost> costs = new List<MaterialCost>();
  public GameObject visualGO;   
  public bool walkable = false;
  public bool takesFullCell = true;
}