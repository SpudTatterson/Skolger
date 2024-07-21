using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ColonistSettingsSO", menuName = "Behaviour Tree/Wander Settings", order = 1)]
public class ColonistSettingsSO : ScriptableObject
{
    [Header("Priority settings")]
    [Header("Higher is more important")]
    [Space]
    public int taskWander;
    public int taskHaul;

    [Header("Wander settings")]
    [Space]
    public float maxWaitTime = 2f;
    public float waypointRange = 10f;

    private void OnValidate() 
    {
        if (taskWander < 0) taskWander = 0;
    }
}