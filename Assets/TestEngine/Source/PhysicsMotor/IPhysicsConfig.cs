using UnityEngine;

namespace TestEngine.Source.PhysicsMotor
{
    public interface IPhysicsConfig
    {
        float GravityForce { get; }
        float MaxFallSpeed  { get; }
        float BounceFactor  { get;}
        LayerMask CollisionMask { get; }
    }
}