using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ColonistMoodManager : MonoBehaviour
{
    ColonistData colonist;

    int maxMood = 100;
    int breakPoint = 10; // the point that the colonist will throw a tantrum
    [SerializeField] int currentMood;
    MoodStatus moodStatus;
    public event Action<int, MoodStatus> onMoodChange;
    public event Action<List<BaseMoodEffect>> OnEffectsChanged;
    public event Action onColonistTantrum;

    [ShowInInspector] Dictionary<MoodModifiers, int> moodModifiers = new Dictionary<MoodModifiers, int>();
    [ShowInInspector] Dictionary<BaseMoodEffectSO, List<BaseMoodEffect>> currentEffects = new Dictionary<BaseMoodEffectSO, List<BaseMoodEffect>>();

    void Awake()
    {
        colonist = GetComponent<ColonistData>();
        moodModifiers.Add(MoodModifiers.Hunger, 0);
    }
    public void ForceMoodUpdate()
    {
        onMoodChange?.Invoke(currentMood, moodStatus);
    }
    public void AddEffect(BaseMoodEffectSO effectData)
    {
        BaseMoodEffect moodEffect = new BaseMoodEffect(effectData);
        if (!effectData.stackable && currentEffects.ContainsKey(effectData) && currentEffects[effectData].Count != 0)
            return;
        if (!currentEffects.ContainsKey(effectData))
        {
            currentEffects.Add(effectData, new List<BaseMoodEffect>());
            currentEffects[effectData].Add(moodEffect);
        }
        OnEffectsChanged?.Invoke(currentEffects[effectData]);
        UpdateCurrentMood(moodEffect.effectData.effect);
    }
    public void RemoveEffect(BaseMoodEffect moodEffect)
    {
        if (!moodEffect.effectData.stackable && currentEffects.ContainsKey(moodEffect.effectData))
        {
            currentEffects[moodEffect.effectData].Remove(moodEffect);
        }
        OnEffectsChanged?.Invoke(currentEffects[moodEffect.effectData]);
        UpdateCurrentMood(-moodEffect.effectData.effect);
    }
    void UpdateCurrentMood(int effect)
    {
        currentMood += effect;
        currentMood = Mathf.Clamp(currentMood, 0, maxMood);
        if (currentMood < breakPoint)
        {
            onColonistTantrum?.Invoke();
        }
        moodStatus = DetermineMoodState(currentMood);
        onMoodChange?.Invoke(currentMood, moodStatus);
    }
    MoodStatus DetermineMoodState(int mood)
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

    public void UpdateMoodModifier(MoodModifiers moodModifierType, int modifier)
    {
        moodModifiers[moodModifierType] = modifier;
        UpdateCurrentMood(modifier);
    }
    public void UpdateMood()
    {
        float timeToAdd = Time.deltaTime;
        foreach (KeyValuePair<BaseMoodEffectSO, List<BaseMoodEffect>> effectType in currentEffects)
        {
            foreach (BaseMoodEffect moodEffect in effectType.Value)
            {
                moodEffect.activeTime += timeToAdd;
                if (moodEffect.activeTime > moodEffect.effectData.effectTime)
                {
                    RemoveEffect(moodEffect);
                }
            }
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