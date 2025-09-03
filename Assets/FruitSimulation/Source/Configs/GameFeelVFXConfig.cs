using UnityEngine;

namespace FruitSimulation.Source.Configs
{
    [CreateAssetMenu(fileName = "Config", menuName = "Configuration/GameFeelVFXConfig", order = 1)]
    public class GameFeelVFXConfig : ScriptableObject
    {
        [Header("Shrinking")]
        [SerializeField] float shrinkFactor = 0.05f;
        [SerializeField] float shrinkDuration = 0.1f;
    
        [Header("Wobble")]
        [SerializeField] float wobbleFactor;
        [SerializeField] float wobbleDuration = 5f;
    
        [Header("Stretch")]
        [SerializeField] float stretchFactor = 0.08f;
        [SerializeField] float stretchDuration = 0.2f;
    
        [Header("Squash")]
        [SerializeField] float squashFactor = 0.04f;
        [SerializeField] float squashDuration = 0.2f;
    
        public float ShrinkFactor => shrinkFactor;
        public float WobbleFactor => wobbleFactor;
        public float StretchFactor => stretchFactor;
        public float SquashFactor => squashFactor;
        public float ShrinkDuration => shrinkDuration;
        public float WobbleDuration => wobbleDuration;
        public float StretchDuration => stretchDuration;
        public float SquashDuration => squashDuration;
    }
}