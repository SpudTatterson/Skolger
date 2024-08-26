using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class HungerManager : MonoBehaviour, IHungerable
{
    ColonistData colonist;
    [field: Header("Hunger")]
    const float Max_Belly_Capacity = 100f;
    [field: SerializeField] public float StuffedThreshold { get; private set; } = 70; // the amount of hunger at which the colonist will be extra happy
    [field: SerializeField] public float HungerThreshold { get; private set; } = 40; // The amount of hungry at which the colonist will drop everything and go eat
    [field: SerializeField, ReadOnly] public float HungerLevel { get; private set; } = 50; // How hungry the colonist current is
    [field: SerializeField] public float HungerGainSpeed { get; private set; } = 0.1f; // Hunger gain per second
    public HungerStatus hungerStatus { get; private set; }
    void Awake()
    {
        colonist = GetComponent<ColonistData>();
    }
    void Start()
    {
        UpdateMood(hungerStatus);
    }
    public void Eat(IEdible edible)
    {
        HungerLevel += edible.FoodValue;
        HungerLevel = Mathf.Clamp(HungerLevel, 0, Max_Belly_Capacity);
    }

    public void GetHungry(float hunger)
    {
        HungerLevel -= HungerGainSpeed * hunger;
        HungerLevel = Mathf.Clamp(HungerLevel, 0, Max_Belly_Capacity);
        HungerStatus hungerStatus = GetHungerStatus();
        if (hungerStatus != this.hungerStatus)
        {
            this.hungerStatus = hungerStatus;
            UpdateMood(hungerStatus);
        }
    }

    void UpdateMood(HungerStatus hungerStatus)
    {
        int moodModifier = GetMoodModifier(hungerStatus);
        colonist.colonistMood.UpdateMoodModifier(MoodModifiers.Hunger, moodModifier);
    }

    int GetMoodModifier(HungerStatus hungerStatus)
    {
        if (hungerStatus == HungerStatus.Starving)
        {
            return -20;
        }
        else if (hungerStatus == HungerStatus.Hungry)
        {
            return -10;
        }
        else if (hungerStatus == HungerStatus.Satisfied)
        {
            return 5;
        }
        else
        {
            return 10;
        }
    }

    HungerStatus GetHungerStatus()
    {
        if (HungerLevel <= 0)
            return HungerStatus.Starving;
        else if (HungerLevel <= HungerThreshold)
            return HungerStatus.Hungry;
        else if (HungerLevel > HungerThreshold && HungerLevel < StuffedThreshold)
            return HungerStatus.Satisfied;
        else
            return HungerStatus.Stuffed;
    }

    public bool IsHungry()
    {
        if (HungerLevel < HungerThreshold) return true;
        return false;
    }

}

public enum HungerStatus
{
    Satisfied,
    Hungry,
    Starving,
    Stuffed
}