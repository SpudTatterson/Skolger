using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistTimeManager : MonoBehaviour
{
    ColonistData colonist;
    [SerializeField] TimeUnit[] hours;


    void Awake()
    {
        if (colonist == null) colonist = GetComponent<ColonistData>();
        if (hours == null || hours.Length != 24) InitializeHours();

    }

    void Start()
    {
        foreach (var hour in hours)
        {
            DayNightEventManager.Instance.GetEvent(hour.time).AddListener(hour.ChangeColonistBrainState);
        }
    }

    void InitializeHours()
    {
        hours = new TimeUnit[24]{
        new TimeUnit(0, BrainState.Rest, colonist),
        new TimeUnit(1, BrainState.Rest, colonist),
        new TimeUnit(2, BrainState.Rest, colonist),
        new TimeUnit(3, BrainState.Rest, colonist),
        new TimeUnit(4, BrainState.Rest, colonist),
        new TimeUnit(5, BrainState.Rest, colonist),
        new TimeUnit(6, BrainState.Rest, colonist),
        new TimeUnit(7, BrainState.Unrestricted, colonist),
        new TimeUnit(9, BrainState.Unrestricted, colonist),
        new TimeUnit(8, BrainState.Unrestricted, colonist),
        new TimeUnit(10, BrainState.Unrestricted, colonist),
        new TimeUnit(11, BrainState.Unrestricted, colonist),
        new TimeUnit(12, BrainState.Unrestricted, colonist),
        new TimeUnit(13, BrainState.Unrestricted, colonist),
        new TimeUnit(14, BrainState.Unrestricted, colonist),
        new TimeUnit(15, BrainState.Unrestricted, colonist),
        new TimeUnit(16, BrainState.Unrestricted, colonist),
        new TimeUnit(17, BrainState.Unrestricted, colonist),
        new TimeUnit(18, BrainState.Unrestricted, colonist),
        new TimeUnit(19, BrainState.Unrestricted, colonist),
        new TimeUnit(20, BrainState.Unrestricted, colonist),
        new TimeUnit(21, BrainState.Unrestricted, colonist),
        new TimeUnit(22, BrainState.Unrestricted, colonist),
        new TimeUnit(23, BrainState.Rest, colonist),
    };
    }

    public void SetNewBrainState(int time, BrainState state)
    {
        hours[time].brainState = state;
    }

    void OnValidate()
    {
        if (colonist == null) colonist = GetComponent<ColonistData>();
        if (hours == null || hours.Length != 24) InitializeHours();
    }
}


[System.Serializable]
class TimeUnit
{
    [SerializeField, HideInInspector] ColonistData colonist;

    [field: SerializeField, HideInInspector] public int time { get; private set; }  
    public BrainState brainState;

    public TimeUnit(int time, BrainState brainState, ColonistData colonist)
    {
        this.time = time;
        this.brainState = brainState;
        this.colonist = colonist;
    }

    public void ChangeColonistBrainState()
    {
        colonist.SetBrainState(brainState);
    }

}