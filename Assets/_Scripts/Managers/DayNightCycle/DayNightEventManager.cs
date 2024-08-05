using System.Collections.Generic;
using UnityEngine;
using Sydewa;
using UnityEngine.Events;

public class DayNightEventManager : MonoSingleton<DayNightEventManager>
{
    [SerializeField] float eventsTolerance = 0.2f;
    [SerializeField, Range(0f, 24f)] float ResetEventsTime = 0.1f;
    [SerializeField] List<EventInfo> events;
    bool DayCycleCompleted;
    Dictionary<float, EventInfo> eventsSortedByTime = new Dictionary<float, EventInfo>();
    Dictionary<string, EventInfo> eventsSortedByNames = new Dictionary<string, EventInfo>();

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

    public UnityEvent GetEvent(string eventName)
    {
        if (eventsSortedByNames.TryGetValue(eventName, out var eventInfo))  
            return eventInfo.Event;
        throw new System.Exception("Failed to find event as none match that name");
    }
    public UnityEvent GetEvent(float timeOfDay)
    {
        if (eventsSortedByTime.TryGetValue(timeOfDay, out var eventInfo))
            return eventInfo.Event;
        throw new System.Exception("Failed to find event as none match the specified time");
    }

    void OnValidate()
    {
        eventsSortedByNames.Clear();
        eventsSortedByTime.Clear();

        foreach (var eventInfo in events)
        {
            if (eventsSortedByNames.ContainsKey(eventInfo.eventName) || eventsSortedByTime.ContainsKey(eventInfo.Time))
            {
                throw new System.Exception("More then one event with the same name or time exist please remove one");
            }
            eventsSortedByNames.Add(eventInfo.eventName, eventInfo);
            eventsSortedByTime.Add(eventInfo.Time, eventInfo);
        }
    }
}

