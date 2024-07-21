using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Placeable/Building")]
public class BuildingData : PlaceableData
{
  [BoxGroup("Prefabs"), Required] public GameObject unplacedVisual;
  [BoxGroup("Prefabs"), Required] public GameObject buildingPrefab;
}
