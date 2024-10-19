using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColonistMoodManager : MonoBehaviour
{
    ColonistData colonist;
    [Header("Settings")]
    [SerializeField] int breakPoint = 10; // the point that the colonist will throw a tantrum
    [SerializeField] int maxMood = 100;
    [SerializeField] Vector2 minMaxBreakdownTimes = new Vector2(30, 100);
    [SerializeField] BaseMoodEffectSO postBreakDownBuff;

    [SerializeField] int currentMood;
    MoodStatus moodStatus;
    public event Action<int, MoodStatus> onMoodChange;
    public event Action<List<BaseMoodEffect>> OnEffectsChanged;
    public event Action onColonistBreakdown;

    [Header("Mood Billboards")]
    [SerializeField] List<BillBoardMood> moodSprites;

    public BreakDownType breakDownType { get; private set; } = BreakDownType.None;

    [ShowInInspector] Dictionary<MoodModifiers, int> moodModifiers = new Dictionary<MoodModifiers, int>();
    [ShowInInspector] Dictionary<BaseMoodEffectSO, List<BaseMoodEffect>> currentEffects = new Dictionary<BaseMoodEffectSO, List<BaseMoodEffect>>();

    void Awake()
    {
        colonist = GetComponent<ColonistData>();

        moodModifiers.Add(MoodModifiers.Hunger, 0);
        colonist.hungerManager.onStatusChange += UpdateHungerModifier;

        moodModifiers.Add(MoodModifiers.Rest, 0);
        colonist.restManger.OnStatusChange += UpdateRestModifier;
        //subscribe to colonist rest on status change

        onMoodChange += UpdateBillboard;
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
        }
        currentEffects[effectData].Add(moodEffect);
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
            // StartBreakdown();
        }
        moodStatus = MoodUtility.DetermineMoodState(currentMood);
        onMoodChange?.Invoke(currentMood, moodStatus);
    }

    void StartBreakdown()
    {
        colonist.SetBrainState(EBrainState.Breakdown);
        onColonistBreakdown?.Invoke();
        float breakDownTime = Random.Range(minMaxBreakdownTimes.x, minMaxBreakdownTimes.y);
        int breakDownType = Random.Range(1, Enum.GetNames(typeof(BreakDownType)).Length);
        this.breakDownType = (BreakDownType)breakDownType;
        Invoke(nameof(StopBreakDown), breakDownTime);
    }

    void StopBreakDown()
    {
        colonist.SetBrainState(EBrainState.Unrestricted);
        breakDownType = BreakDownType.None;
        AddEffect(postBreakDownBuff);
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
            for (int i = 0; i < effectType.Value.Count; i++)
            {
                BaseMoodEffect moodEffect = effectType.Value[i];
                moodEffect.activeTime += timeToAdd;
                if (moodEffect.activeTime > moodEffect.effectData.effectTime)
                {
                    RemoveEffect(moodEffect);
                }
            }
        }

    }
    void UpdateHungerModifier(HungerStatus status)
    {
        UpdateMoodModifier(MoodModifiers.Hunger, MoodUtility.GetMoodModifier(status));
    }
    void UpdateRestModifier(RestStatus status)
    {
        UpdateMoodModifier(MoodModifiers.Rest, MoodUtility.GetMoodModifier(status));
    }

    void UpdateBillboard(int mood, MoodStatus status)
    {
        colonist.billboard.gameObject.SetActive(true);
        colonist.billboard.UpdateImage(moodSprites.Find(x => x.Status == status).sprite);
        Invoke("TurnBillboardOff", 10);
    }
    void TurnBillboardOff()
    {
        colonist.billboard.gameObject.SetActive(false);
    }
}

[Serializable]
class BillBoardMood
{
    public MoodStatus Status;
    public Sprite sprite;
}