using UnityEngine;

public class ColonistData : MonoBehaviour, IHungerable
{
    const float Max_Belly_Capacity = 100f;
    [field:SerializeField]public float HungerThreshold { get; private set; } = 40;
    [field:SerializeField]public float HungerLevel { get; private set; } = 50;
    [field:SerializeField]public float HungerGainSpeed {get; private set; } = 1;

    public void Eat(IEdible edible)
    {
        HungerLevel += edible.FoodValue;
        Mathf.Clamp(HungerLevel, 0, Max_Belly_Capacity);
    }

    public void GetHungry(float hunger)
    {
        HungerLevel -= HungerGainSpeed * hunger;
        Mathf.Clamp(HungerLevel, 0 , Max_Belly_Capacity);
    }

    public bool IsHungry()
    {
        if (HungerLevel < HungerThreshold) return true;
        return false;
    }   
    
    void Update()
    {
        GetHungry(Time.deltaTime);
    }
}