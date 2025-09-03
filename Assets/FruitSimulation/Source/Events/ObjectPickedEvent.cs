using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Events
{
    public struct ObjectPickedEvent : IEvent
    {
        public Transform TargetTransform { get;  private set; }
        public Transform RootTransform { get;  private set; }
        
        public ObjectPickedEvent(Transform targetTransform, Transform rootTransform)
        {
            TargetTransform = targetTransform;
            RootTransform  = rootTransform;
        }
    }
}