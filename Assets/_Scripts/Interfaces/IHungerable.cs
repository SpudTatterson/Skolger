using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHungerable
{
    float HungerLevel { get; }
    float HungerThreshold { get; }
    float HungerGainSpeed { get; }

    void Eat(IEdible edible);
    bool IsHungry();
    void GetHungry(float hunger);
}
