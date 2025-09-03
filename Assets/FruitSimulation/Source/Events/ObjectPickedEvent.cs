using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Events
{
    public struct ObjectPickedEvent : IEvent
    {
        public Transform Transform { get;  private set; }

        public ObjectPickedEvent(Transform transform)
        {
            Transform = transform;
        }
    }
}