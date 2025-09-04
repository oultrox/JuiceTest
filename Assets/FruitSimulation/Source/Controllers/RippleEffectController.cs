using FruitSimulation.Source.Events;
using SimpleBus;
using UnityEngine;

/// <summary>
/// Flex component. Uses very old ripple effect!
/// </summary>
public class RippleEffectController : MonoBehaviour
{
    RippleEffect _rippleEffect;
    EventListener<ObjectDestroyedEvent> _objectDestroyedListener;
    

    void Start()
    {
        if (Camera.main != null) _rippleEffect = Camera.main.gameObject.GetComponent<RippleEffect>();
        _objectDestroyedListener = new EventListener<ObjectDestroyedEvent>(TriggerRipple);
        EventBus<ObjectDestroyedEvent>.Register(_objectDestroyedListener);
    }

    void TriggerRipple(ObjectDestroyedEvent obj)
    {
        _rippleEffect?.SpawnRippleAtWorldPosition(obj.Position);
    }
}
