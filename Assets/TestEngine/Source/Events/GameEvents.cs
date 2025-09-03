using SimpleBus;
using UnityEngine;

namespace TestEngine.Source.Events
{
    public struct ObjectPickedEvent : IEvent
    {
        public Transform Transform { get;  private set; }

        public ObjectPickedEvent(Transform transform)
        {
            Transform = transform;
        }
    }
    
    public struct ObjectDroppedEvent : IEvent
    {
        public Vector2 Velocity { get;  private set; }
        public Transform Transform { get;  private set; }
        public ObjectDroppedEvent(Vector2 velocity, Transform transform)
        {
            Velocity = velocity;
            Transform = transform;
        }
    }
}