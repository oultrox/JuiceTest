using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Events
{
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