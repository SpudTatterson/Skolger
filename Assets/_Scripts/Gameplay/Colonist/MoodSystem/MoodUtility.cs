using System;
using UnityEngine;

public static class MoodUtility
{
    public static int GetMoodModifier(HungerStatus hungerStatus)
    {
        switch (hungerStatus)
        {
            case HungerStatus.Starving:
                return -20;
            case HungerStatus.Hungry:
                return -10;
            case HungerStatus.Satisfied:
                return 5;
            default:
                return 10;
        }
    }
    public static int GetMoodModifier(RestStatus restStatus)
    {
        switch (restStatus)
        {
            case RestStatus.Insomniac: return -20;
            case RestStatus.SleepDeprived: return -10;
            case RestStatus.Tired: return -5;
            case RestStatus.Rested: return 5;
            case RestStatus.WellRested: return 15;
        }
        throw new NotImplementedException("Missing case for RestStatue enum option");
    }

    public static MoodStatus DetermineMoodState(int mood)
    {
        if (mood >= 80f)
        {
            return MoodStatus.Happy;
        }
        else if (mood >= 60f)
        {
            return MoodStatus.Content;
        }
        else if (mood >= 40f)
        {
            return MoodStatus.Neutral;
        }
        else if (mood >= 20f)
        {
            return MoodStatus.Stressed;
        }
        else
        {
            return MoodStatus.Depressed;
        }
    }
}

public enum MoodModifiers
{
    Health,
    Hunger,
    Rest
}
public enum MoodStatus
{
    Happy,
    Content,
    Neutral,
    Stressed,
    Depressed
}

public enum BreakDownType
{
    None,
    Wander,
    EatingFrenzy,
}