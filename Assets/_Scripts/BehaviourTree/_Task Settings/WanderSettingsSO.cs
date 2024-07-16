using UnityEngine;

[CreateAssetMenu(fileName = "WanderSettings", menuName = "Behaviour Tree/Wander Settings", order = 1)]
public class WanderSettingsSO : ScriptableObject
{
    [Header("Wander Settings")]
    public float maxWaitTime = 2f;
    public float waypointRange = 10f;
    public int priority;
}