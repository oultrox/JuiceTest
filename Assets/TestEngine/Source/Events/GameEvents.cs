using SimpleBus;
using UnityEngine;

namespace TestEngine.Source.Events
{
    public struct ObjectPickedEvent : IEvent
    {
        public Transform ObjectTransform { get;  private set; }

        public ObjectPickedEvent(Transform transform)
        {
            ObjectTransform = transform;
        }
    }
}