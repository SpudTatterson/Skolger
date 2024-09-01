using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/BasicBuilding")]
public class BuildingData : PlaceableData
{
  [BoxGroup("Prefabs"), Required] public GameObject unplacedVisual;
  [BoxGroup("Prefabs"), Required] public GameObject buildingPrefab;
}
