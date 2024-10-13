using Skolger.Tutorial;
using UnityEngine;
using UnityEngine.Events;

namespace Skolger.Tutorial

{
    public class EventVisualAid : VisualAid
    {
        [SerializeField] UnityEvent taskStartEvent;
        [SerializeField] UnityEvent taskEndEvent;
        public override void Initialize()
        {
            taskStartEvent?.Invoke();
        }

        public override void Reset()
        {
            taskEndEvent?.Invoke();
        }

        public override void Update()
        {
        
        }
    }
}