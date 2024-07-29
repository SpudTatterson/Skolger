using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ColonistSettingsSO", menuName = "Behaviour Tree/Wander Settings", order = 1)]
public class ColonistSettingsSO : ScriptableObject
{
    [Header("Wander settings")]
    [Space]
    public float maxWaitTime = 2f;
    public float waypointRange = 10f;
}