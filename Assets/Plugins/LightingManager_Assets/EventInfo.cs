using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sydewa
{
    [System.Serializable]
    public class EventInfo
    {
        public string eventName;
        [Range(0, 24)] public float Time;
        public UnityEvent Event;
        public bool executed;

        public EventInfo(string eventName, float time)
        {
            this.eventName = eventName;
            Time = time;
            Event = new UnityEvent();
            executed = false;
        }
    }
}
