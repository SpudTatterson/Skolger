using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ColonistSettingsSO", menuName = "Behaviour Tree/Wander Settings", order = 1)]
public class ColonistSettingsSO : ScriptableObject
{
    [Header("Priority settings")]
    [Header("Higher is more important")]
    [Space]
    public int priorityWander;
    public int priorityPickUpItem;
    [ReadOnly] public int priorityHaulToStockpile;

    [Header("Wander settings")]
    [Space]
    public float maxWaitTime = 2f;
    public float waypointRange = 10f;

    private void OnValidate() 
    {
        if (priorityWander < 0) priorityWander = 0;
        if (priorityPickUpItem < 0) priorityPickUpItem = 0;
        if (priorityHaulToStockpile < 0) priorityHaulToStockpile = 0;

        priorityHaulToStockpile = priorityPickUpItem + 1;
    }
}