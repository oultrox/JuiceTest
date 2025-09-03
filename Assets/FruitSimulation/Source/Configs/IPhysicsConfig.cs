using UnityEngine;

namespace FruitSimulation.Source.Configs
{
    public interface IPhysicsConfig
    {
        float GravityForce { get; }
        float MaxFallSpeed  { get; }
        float BounceFactor  { get;}
        LayerMask CollisionMask { get; }
        float Deceleration { get; }
    }
}