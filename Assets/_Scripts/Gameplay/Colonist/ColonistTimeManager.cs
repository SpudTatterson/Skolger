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
        new TimeUnit(0, EBrainState.Rest, colonist),
        new TimeUnit(1, EBrainState.Rest, colonist),
        new TimeUnit(2, EBrainState.Rest, colonist),
        new TimeUnit(3, EBrainState.Rest, colonist),
        new TimeUnit(4, EBrainState.Rest, colonist),
        new TimeUnit(5, EBrainState.Rest, colonist),
        new TimeUnit(6, EBrainState.Rest, colonist),
        new TimeUnit(7, EBrainState.Unrestricted, colonist),
        new TimeUnit(9, EBrainState.Unrestricted, colonist),
        new TimeUnit(8, EBrainState.Unrestricted, colonist),
        new TimeUnit(10, EBrainState.Unrestricted, colonist),
        new TimeUnit(11, EBrainState.Unrestricted, colonist),
        new TimeUnit(12, EBrainState.Unrestricted, colonist),
        new TimeUnit(13, EBrainState.Unrestricted, colonist),
        new TimeUnit(14, EBrainState.Unrestricted, colonist),
        new TimeUnit(15, EBrainState.Unrestricted, colonist),
        new TimeUnit(16, EBrainState.Unrestricted, colonist),
        new TimeUnit(17, EBrainState.Unrestricted, colonist),
        new TimeUnit(18, EBrainState.Unrestricted, colonist),
        new TimeUnit(19, EBrainState.Unrestricted, colonist),
        new TimeUnit(20, EBrainState.Unrestricted, colonist),
        new TimeUnit(21, EBrainState.Unrestricted, colonist),
        new TimeUnit(22, EBrainState.Unrestricted, colonist),
        new TimeUnit(23, EBrainState.Rest, colonist),
    };
    }

    public void SetNewBrainState(int time, EBrainState state)
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
    public EBrainState brainState;

    public TimeUnit(int time, EBrainState brainState, ColonistData colonist)
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