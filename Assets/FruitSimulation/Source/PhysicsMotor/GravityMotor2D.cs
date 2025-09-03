using System;
using CustomRaycastEngine;
using TestEngine.Source.PhysicsMotor;
using UnityEngine;

namespace FruitSimulation.Source.PhysicsMotor
{
    /// <summary>
    /// Engine motor for movement-related entities.
    /// </summary>
    public class GravityMotor2D : RaycastMotor2D
    {
        struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;
            public int faceDir;

            public void Reset()
            {
                above = below = left = right = false;
            }
        }
        
        public Action OnBounce;
        const float MIN_BOUNCE_VELOCITY = 0.9f;
        float _gravity;
        float _maxFallSpeed; 
        float _bounceFactor;
        float _deceleration;
        bool _isActive;
        CollisionInfo _collisionInfo;
        LayerMask _collisionMask;
        Transform _transform;
        Vector2 _velocity;
        
        
        public GravityMotor2D(Transform transform, BoxCollider2D collider, IPhysicsConfig config) : base(collider)
        {
            _transform = transform;
            _collisionInfo.faceDir = 1;
            _collisionMask = config.CollisionMask;
            _gravity = config.GravityForce;
            _maxFallSpeed = config.MaxFallSpeed;
            _bounceFactor = config.BounceFactor;
            _deceleration = config.Deceleration;
        }

        /// <summary>
        /// Enables ticking for the raycast motor engine.
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            _isActive = active;
        }
        
        /// <summary>
        /// Optional: set initial velocity for throwing objects.
        /// </summary>
        public void SetVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        /// <summary>
        /// Executes the motor engine calculations for raycasts. Includes Gravity and raycast orientations.
        /// </summary>
        /// <param name="dt"></param>
        public void Tick(float dt)
        {
            if(!_isActive) return;
            ApplyGravity(ref _velocity);
            ApplyHorizontalDeceleration(ref _velocity, dt);
            ApplyMovement(dt);
            SnapVertical(ref _velocity);
        }
        
        void ApplyMovement(float dt)
        {
            Vector2 move = _velocity * dt;
            Move(move);
        }

        void Move(Vector2 moveAmount, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();
            _collisionInfo.Reset();

            if (moveAmount.x != 0) _collisionInfo.faceDir = (int)Mathf.Sign(moveAmount.x);

            CheckHorizontalCollisions(ref moveAmount);
            if (moveAmount.y != 0) CheckVerticalCollisions(ref moveAmount);

            // Snap to floor before translation
            if (_collisionInfo.below && Math.Abs(moveAmount.y) < MIN_BOUNCE_VELOCITY)
                moveAmount.y = 0;

            _transform.Translate(moveAmount);

            if (standingOnPlatform) _collisionInfo.below = true;
        }
        
        void SnapVertical(ref Vector2 velocity)
        {
            // Bounce only on the frame you *just* hit the ground
            if (_collisionInfo.below && velocity.y < 0)
            {
                if (Mathf.Abs(velocity.y) > MIN_BOUNCE_VELOCITY)
                {
                    velocity.y = -velocity.y * _bounceFactor;
                    OnBounce?.Invoke();
                }
                else
                {
                    velocity.y = 0f; // settle on ground
                }
            }
        }
        
        void ApplyGravity(ref Vector2 velocity)
        {
            velocity.y += _gravity * Time.deltaTime;
            if (velocity.y < _maxFallSpeed)
                velocity.y = _maxFallSpeed;
        }
        
        void ApplyHorizontalDeceleration(ref Vector2 velocity, float dt)
        {
            bool isBouncing = _collisionInfo.left || _collisionInfo.right;
            if (isBouncing) return; 
            if (Mathf.Approximately(velocity.x, 0)) return;

            float decel = _deceleration * dt;
            if (Mathf.Abs(velocity.x) <= decel)
            {
                velocity.x = 0;
            }
            else
            {
                velocity.x -= Mathf.Sign(velocity.x) * decel;
            }
        }

        /// <summary>
        /// Checks horizontal collisions separately for more precise wall calculations, applying bounce factors if possible.
        /// </summary>
        /// <param name="moveAmount"></param>
        void CheckHorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = _collisionInfo.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + SKIN_WIDTH;

            if (Mathf.Abs(moveAmount.x) < SKIN_WIDTH)
                rayLength = SKIN_WIDTH;

            for (int i = 0; i < HorizontalRayCount; i++)
            {
                Vector2 rayOrigin = GetHorizontalRayOrigin(Mathf.Approximately(directionX, -1), i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (!hit) continue;

                // Snap movement flush against wall
                moveAmount.x = (hit.distance - SKIN_WIDTH) * directionX;

                // Only bounce if velocity is large enough
                if (Mathf.Abs(_velocity.x) > MIN_BOUNCE_VELOCITY)
                {
                    _velocity.x = -_velocity.x * _bounceFactor; // use actual velocity
                    OnBounce?.Invoke();

                    // Recalculate moveAmount for this frame
                    moveAmount.x = _velocity.x * Time.deltaTime;
                }
                else
                {
                    _velocity.x = 0;
                }

                _collisionInfo.left = Mathf.Approximately(directionX, -1);
                _collisionInfo.right = Mathf.Approximately(directionX, 1);

                break; // stop checking after first hit
            }
        }
        
        /// <summary>
        /// Checks vertical collisions separately for more precise ground calculations, applying bounce factors if possible.
        /// </summary>
        /// <param name="moveAmount"></param>
        void CheckVerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + SKIN_WIDTH;
            Vector2 rayOrigin;
            RaycastHit2D raycastHit;

            // Cast vertical rays to detect collisions above/below.
            for (int i = 0; i < VerticalRayCount; i++)
            {
                rayOrigin = (Mathf.Approximately(directionY, -1)) ? raycastOrigin.bottomLeft : raycastOrigin.topLeft;
                rayOrigin += Vector2.right * (VerticalRaySpacing * i + moveAmount.x);

                raycastHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (!raycastHit)
                {
                    continue;
                }
                
                // Apply vertical adjustment.
                moveAmount.y = (raycastHit.distance - SKIN_WIDTH) * directionY;
                rayLength = raycastHit.distance;
            

                // Update collision state above/below.
                _collisionInfo.below = Mathf.Approximately(directionY, -1);
                _collisionInfo.above = Mathf.Approximately(directionY, 1);
            }
        }
    }
}
