using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Skolger.Tutorial
{
    public class DynamicEventStep : BaseStep
    {
        [SerializeField] UnityEngine.Object targetObject;

        [SerializeField, ValueDropdown("GetEventNames")] string selectedEvent;

        EventInfo targetEvent;
        Delegate eventDelegate;

        // Method to dynamically get the list of events from the targetObject
        IEnumerable<string> GetEventNames()
        {
            if (targetObject == null) return Enumerable.Empty<string>();

            var events = targetObject.GetType()
                                     .GetEvents(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(e => IsSupportedEventType(e.EventHandlerType)); // Check if the event type is supported

            return events.Select(e => e.Name);
        }

        // Check if the event type is supported (Action, Action<T>, Action<T1, T2>)
        bool IsSupportedEventType(Type eventType)
        {
            if (eventType == typeof(Action)) return true;
            if (eventType.IsGenericType && eventType.GetGenericTypeDefinition() == typeof(Action<>)) return true;
            if (eventType.IsGenericType && eventType.GetGenericTypeDefinition() == typeof(Action<,>)) return true;

            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (targetObject == null || string.IsNullOrEmpty(selectedEvent))
            {
                Debug.LogError("No target object or event selected.");
                return;
            }

            // Get the selected event from the target object
            targetEvent = targetObject.GetType().GetEvent(selectedEvent);
            if (targetEvent == null)
            {
                Debug.LogError($"Event {selectedEvent} not found on {targetObject.name}");
                return;
            }
            // Subscribe to the event, dynamically handling Action events
            SubscribeToEvent(targetEvent);
        }

        // Subscribe to the event based on its type
        void SubscribeToEvent(EventInfo eventInfo)
        {
            Type eventHandlerType = eventInfo.EventHandlerType;

            if (eventHandlerType == typeof(Action))
            {
                // Handle basic Action events
                eventDelegate = (Action)OnEventTriggered;
            }
            else if (eventHandlerType.IsGenericType && eventHandlerType.GetGenericTypeDefinition() == typeof(Action<>))
            {
                // Handle Action<T> events
                MethodInfo method = GetType().GetMethod(nameof(OnEventTriggeredGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                                           .MakeGenericMethod(eventHandlerType.GetGenericArguments());
                eventDelegate = Delegate.CreateDelegate(eventHandlerType, this, method);
            }
            else if (eventHandlerType.IsGenericType && eventHandlerType.GetGenericTypeDefinition() == typeof(Action<,>))
            {
                // Handle Action<T1, T2> events
                MethodInfo method = GetType().GetMethod(nameof(OnEventTriggeredDoubleGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                                           .MakeGenericMethod(eventHandlerType.GetGenericArguments());
                eventDelegate = Delegate.CreateDelegate(eventHandlerType, this, method);
            }

            // Add the event handler
            if (eventDelegate != null)
            {
                eventInfo.AddEventHandler(targetObject, eventDelegate);
            }
        }

        // Event handler for Action without parameters
        void OnEventTriggered()
        {
            Finish();
        }

        // Generic event handler for Action<T>
        void OnEventTriggeredGeneric<T>(T arg)
        {
            Finish();
        }

        // Generic event handler for Action<T1, T2>
        void OnEventTriggeredDoubleGeneric<T1, T2>(T1 arg1, T2 arg2)
        {
            Finish();
        }

        public override void Finish()
        {
            base.Finish();

            if (targetEvent != null && eventDelegate != null)
            {
                targetEvent.RemoveEventHandler(targetObject, eventDelegate);
            }
        }
    }


}

