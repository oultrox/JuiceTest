using FruitSimulation.Source.Configs;
using FruitSimulation.Source.Events;
using SimpleBus;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] PhysicsConfig _physicsConfig;
    ObjectDestroyedEvent _objectDestroyedEvent;
    ObjectDroppedEvent _objectDroppedEvent;
    Vector2 boxSize = Vector2.one;
    readonly Vector2 _boxOffset = Vector2.zero;

    Collider2D _collider;
    Collider2D[] _hits = new Collider2D[10]; // adjust size for max expected objects per frame

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        boxSize = _collider.bounds.size;
        _objectDestroyedEvent = new ObjectDestroyedEvent(transform.position);
    }

    void Update()
    {
        CheckRayForCollisions();
    }

    void CheckRayForCollisions()
    {
        Vector2 center = (Vector2)transform.position + _boxOffset;
        int hitCount = Physics2D.OverlapBoxNonAlloc(center, boxSize, 0f, _hits, _physicsConfig.GrabbableMask.value);

        for (int i = 0; i < hitCount; i++)
        {
            var obj = _hits[i];
            Destroy(obj.gameObject);
            EventBus<ObjectDestroyedEvent>.Raise(_objectDestroyedEvent);
            EventBus<ObjectDroppedEvent>.Raise(new ObjectDroppedEvent(Vector2.zero, obj.transform));
        }
    }

    // Optional: visualize the box in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + _boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}
