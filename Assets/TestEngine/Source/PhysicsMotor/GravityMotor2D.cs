using System;
using UnityEngine;

namespace TestEngine.Source.PhysicsMotor
{
    /// <summary>
    /// Engine motor for movement-related gravity entities.
    /// </summary>
    public class GravityMotor2D : RaycastMotor2D
    {
        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;
            public int faceDir;
            public Vector2 velocityOld;
            public Vector2 slopeNormal;
            public Collider2D platformStanding;

            public void Reset()
            {
                above = below = left = right = false;
                slopeNormal = Vector2.zero;
            }
        }
        
        const float MIN_BOUNCE_VELOCITY = 0.9f;
        float _gravity;
        float _maxFallSpeed; 
        float _bounceFactor;
        float _deceleration;
        CollisionInfo collisionInfo;
        LayerMask collisionMask;
        Transform _transform;
        Vector2 _velocity;
        
        public CollisionInfo CollisionData => collisionInfo;
        public Vector2 GetVelocity() => _velocity;
        public Action OnBounce;
        
        public GravityMotor2D(Transform transform, BoxCollider2D collider, IPhysicsConfig config) : base(collider)
        {
            _transform = transform;
            collisionInfo.faceDir = 1;
            collisionMask = config.CollisionMask;
            _gravity = config.GravityForce;
            _maxFallSpeed = config.MaxFallSpeed;
            _bounceFactor = config.BounceFactor;
            _deceleration = config.Deceleration; // e.g., 15f
        }
        
        /// <summary>
        /// Optional: set initial velocity for throwing objects.
        /// </summary>
        public void SetVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public void Tick(float dt)
        {
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
            collisionInfo.Reset();
            collisionInfo.velocityOld = moveAmount;

            if (moveAmount.x != 0) collisionInfo.faceDir = (int)Mathf.Sign(moveAmount.x);

            CheckHorizontalCollisions(ref moveAmount);
            if (moveAmount.y != 0) CheckVerticalCollisions(ref moveAmount);

            // Snap to floor before translation
            if (collisionInfo.below && Math.Abs(moveAmount.y) < MIN_BOUNCE_VELOCITY)
                moveAmount.y = 0;

            _transform.Translate(moveAmount);

            if (standingOnPlatform) collisionInfo.below = true;
        }
        
        void SnapVertical(ref Vector2 velocity)
        {
            // Bounce only on the frame you *just* hit the ground
            if (collisionInfo.below && velocity.y < 0)
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
            // Apply gravity
            velocity.y += _gravity * Time.deltaTime;
            if (velocity.y < _maxFallSpeed)
                velocity.y = _maxFallSpeed;
        }

        /// <summary>
        /// Checks horizontal and vertical collisions separately for more precise slope/ground calculations.
        /// </summary>
        /// <param name="moveAmount"></param>
        void CheckHorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisionInfo.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + SKIN_WIDTH;

            if (Mathf.Abs(moveAmount.x) < SKIN_WIDTH)
                rayLength = SKIN_WIDTH;

            for (int i = 0; i < HorizontalRayCount; i++)
            {
                Vector2 rayOrigin = GetHorizontalRayOrigin(Mathf.Approximately(directionX, -1), i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (!hit) continue;

                // Snap movement flush against wall
                moveAmount.x = (hit.distance - SKIN_WIDTH) * directionX;

                // Only bounce if velocity is large enough
                if (Mathf.Abs(_velocity.x) > 0.5f)
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

                collisionInfo.left = Mathf.Approximately(directionX, -1);
                collisionInfo.right = Mathf.Approximately(directionX, 1);

                break; // stop checking after first hit
            }
        }
        
        void ApplyHorizontalDeceleration(ref Vector2 velocity, float dt)
        {
            if (collisionInfo.left || collisionInfo.right) return; // skip deceleration when bouncing
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

                raycastHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (!raycastHit)
                {
                    continue;
                }
            
                // Store reference to standing platform collider.
                collisionInfo.platformStanding = raycastHit.collider;

                // Apply vertical adjustment.
                moveAmount.y = (raycastHit.distance - SKIN_WIDTH) * directionY;
                rayLength = raycastHit.distance;
            

                // Update collision state above/below.
                collisionInfo.below = Mathf.Approximately(directionY, -1);
                collisionInfo.above = Mathf.Approximately(directionY, 1);
            }
        }
    }
}
