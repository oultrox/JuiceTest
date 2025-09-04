using UnityEngine;

namespace FruitSimulation.Source.Configs
{
    /// <summary>
    /// Defines the data contract for physics configuration values used by objects that rely on a physics motor.
    /// </summary>
    public interface IPhysicsConfig
    {
        float GravityForce { get; }
        float MaxFallSpeed  { get; }
        float BounceFactor  { get;}
        LayerMask CollisionMask { get; }
        LayerMask GrabbableMask { get; }
        float Deceleration { get; }
    }
}