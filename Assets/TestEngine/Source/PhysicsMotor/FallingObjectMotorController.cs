using SimpleBus;
using TestEngine.Source.Events;
using TestEngine.Source.Juice;
using UnityEngine;

namespace TestEngine.Source.PhysicsMotor
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FallingObjectMotorController : MonoBehaviour
    {
        [SerializeField] PhysicsConfig physicsConfig;
        EventListener<ObjectPickedEvent> _onObjectPicked;
        EventListener<ObjectDroppedEvent> _onObjectDropped;
        GravityMotor2D _motor;
        BoxCollider2D _collider;
        Vector2 _velocity;
        AnimatorService _animator;
        SpriteRenderer _spriteRenderer;
        bool _isActive;

        
        private void Awake()
        {
            InitBehaviors();
        }
        
        void Update()
        {
            if (!_isActive) return;
            _motor.Tick(Time.deltaTime);
        }
        
        void InitBehaviors()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _motor = new GravityMotor2D(transform, _collider, physicsConfig);
            _animator = new AnimatorService(this, _spriteRenderer);
            _onObjectPicked = new EventListener<ObjectPickedEvent>(CheckPickedUp);
            _onObjectDropped = new EventListener<ObjectDroppedEvent>(LaunchObject);
            _motor.OnBounce += BounceSprite;
            EventBus<ObjectPickedEvent>.Register(_onObjectPicked);
            EventBus<ObjectDroppedEvent>.Register(_onObjectDropped);
        }

        void LaunchObject(ObjectDroppedEvent obj)
        {
            _isActive = true;
            _motor.SetVelocity(obj.Velocity);
        }

        void CheckPickedUp(ObjectPickedEvent obj)
        {
            if (obj.ObjectTransform != transform) return;
            
            _isActive = false;
            transform.parent = null;
            _animator.PlayStretch(0.08f);
        }
        
        void BounceSprite()
        {
            _animator.PlaySquash(0.04f);
        }
    }
}