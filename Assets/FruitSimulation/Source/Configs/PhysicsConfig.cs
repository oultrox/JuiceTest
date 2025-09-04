using UnityEngine;
using UnityEngine.Serialization;

namespace FruitSimulation.Source.Configs
{
    [CreateAssetMenu(fileName = "Config", menuName = "Configuration/PhysicsConfig", order = 1)]
    public class PhysicsConfig : ScriptableObject, IPhysicsConfig
    {
        [SerializeField] float gravityForce = -20 ;
        [SerializeField] float maxFallSpeed = -30;
        [Range(0f, 1f)] [SerializeField] float bounceFactor = 0.5f;
        [SerializeField] LayerMask collisionMask;
        [SerializeField] LayerMask grabbableMask;
        [SerializeField] float decceleration;
        
        public float GravityForce => gravityForce;
        public float MaxFallSpeed => maxFallSpeed;
        public float BounceFactor => bounceFactor;
        public LayerMask CollisionMask => collisionMask;
        public LayerMask GrabbableMask => grabbableMask;
        public float Deceleration => decceleration;
    }
}