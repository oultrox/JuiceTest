using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Events
{
    public struct ObjectDestroyedEvent : IEvent
    {
        public Vector2 Position { get;  private set; }
        public ObjectDestroyedEvent(Vector2 position)
        {
            Position = position;
        }
    }
}