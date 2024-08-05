using System.Collections.Generic;
using UnityEngine;
using Sydewa;

public class DayNightEventManager : MonoBehaviour
{
    [SerializeField] float eventsTolerance = 0.2f;
    [SerializeField, Range(0f, 24f)] float ResetEventsTime = 0.1f;
    [SerializeField] List<EventInfo> events;
    bool DayCycleCompleted;

    public void Initialize(List<EventInfo> eventList)
    {
        events = eventList;
        ResetEvents();
    }
    void Update()
    {
        CheckEvents(DayNightTimeManager.Instance.TimeOfDay);
    }
    public void CheckEvents(float TimeOfDay)
    {
        foreach (var eventInfo in events)
        {
            float timeDifference = Mathf.Abs(eventInfo.Time - TimeOfDay);
            if (timeDifference <= eventsTolerance && !eventInfo.executed)
            {
                eventInfo.executed = true;
                eventInfo.Event.Invoke();
                Debug.Log("Event: " + eventInfo.eventName);
            }
        }

        if (!DayCycleCompleted && TimeOfDay < ResetEventsTime)
        {
            DayCycleCompleted = true;
            ResetEvents();
            Debug.Log("Day completed + reset");
        }
        else if (TimeOfDay > ResetEventsTime)
        {
            DayCycleCompleted = false;
        }
    }

    void ResetEvents()
    {
        foreach (var eventInfo in events)
        {
            eventInfo.executed = false;
        }
    }
}

