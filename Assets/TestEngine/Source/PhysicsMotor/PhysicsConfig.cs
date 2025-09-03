using TestEngine.Source.PhysicsMotor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PhysicsConfig", menuName = "Configuration/PhysicsConfig", order = 1)]
public class PhysicsConfig : ScriptableObject, IPhysicsConfig
{
    [SerializeField] float gravityForce = -20 ;
    [SerializeField] float maxFallSpeed = -30;
    [Range(0f, 1f)] [SerializeField] float bounceFactor = 0.5f;
    [SerializeField] LayerMask collisionMask;
    [FormerlySerializedAs("deccaceleration")] [SerializeField] float decceleration;
    public float GravityForce => gravityForce;
    public float MaxFallSpeed => maxFallSpeed;
    public float BounceFactor => bounceFactor;
    public LayerMask CollisionMask => collisionMask;
    public float Deceleration => decceleration;
}