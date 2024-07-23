using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ColonistSettingsSO", menuName = "Behaviour Tree/Wander Settings", order = 1)]
public class ColonistSettingsSO : ScriptableObject
{
    [Header("Priority settings\nHigher is more important")]
    [Header("Basic tasks with not control")]
    [Space]
    public int taskWander;
    public int taskEat;

    [Header("Player tasks that change in run time")]
    [Space]
    public int taskHaul;
    public int taskConstruction;
    public int taskHarvest;

    [Header("Wander settings")]
    [Space]
    public float maxWaitTime = 2f;
    public float waypointRange = 10f;

    private void OnValidate() 
    {
        if (taskWander < 0) taskWander = 0;
        if (taskEat < 0) taskEat = 11;

        if (taskHaul < 1) taskHaul = 1;
        if (taskConstruction < 1) taskConstruction = 1;
        if (taskHarvest < 1) taskHarvest = 1;
    }
}