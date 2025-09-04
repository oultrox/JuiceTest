using System;
using FruitSimulation.Source.Animators;
using FruitSimulation.Source.Configs;
using FruitSimulation.Source.Events;
using FruitSimulation.Source.PhysicsMotor;
using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Controllers
{
    /// <summary>
    /// Controls any falling object that gets this component attached, executing engine and code-driven animations.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class FallingObjectController : MonoBehaviour
    {
        [Header ("Injection")]
        [SerializeField] GameFeelVFXConfig juiceConfig;
        [SerializeField] PhysicsConfig physicsConfig;
        
        EventListener<ObjectPickedEvent> _onObjectPicked;
        EventListener<ObjectDroppedEvent> _onObjectDropped;
        GravityMotor2D _motor;
        BoxCollider2D _collider;
        Vector2 _velocity;
        FruitAnimator _animator;
        SpriteRenderer _spriteRenderer;
        
        
        void Awake()
        {
            InitBehaviors();
        }
        
        void Update()
        {
            _motor.Tick(Time.deltaTime);
        }
        
        void OnDisable()
        {
            EventBus<ObjectPickedEvent>.Deregister(_onObjectPicked);
            EventBus<ObjectDroppedEvent>.Deregister(_onObjectDropped);
        }
        
        void InitBehaviors()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            
            _motor = new GravityMotor2D(transform, _collider, physicsConfig);
            _motor.OnBounce += BounceSprite;
            
            _animator = new FruitAnimator(this, _spriteRenderer, juiceConfig);
            _animator.PlayShrink();
            
            _onObjectPicked = new EventListener<ObjectPickedEvent>(CheckPickedUp);
            _onObjectDropped = new EventListener<ObjectDroppedEvent>(LaunchObject);
            
            EventBus<ObjectPickedEvent>.Register(_onObjectPicked);
            EventBus<ObjectDroppedEvent>.Register(_onObjectDropped);
        }
        
        void LaunchObject(ObjectDroppedEvent obj)
        {
            if (obj.Transform != transform) return;
            
            _motor.SetActive(true);
            _motor.SetVelocity(obj.Velocity);
        }

        void CheckPickedUp(ObjectPickedEvent obj)
        {
            if (obj.TargetTransform != transform) return;
            _motor.SetActive(false);
            _animator.PlayStretch();
        }
        
        void BounceSprite()
        {
            _animator.PlaySquash();
        }
    }
}