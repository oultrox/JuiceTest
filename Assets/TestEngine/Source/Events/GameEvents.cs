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
    
    public struct ObjectDroppedEvent : IEvent
    {
        public Vector2 Velocity { get;  private set; }

        public ObjectDroppedEvent(Vector2 velocity)
        {
            Velocity = velocity;
        }
    }
}