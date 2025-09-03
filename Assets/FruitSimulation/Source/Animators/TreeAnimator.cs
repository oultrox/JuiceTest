using AnimationService;
using FruitSimulation.Source.Configs;
using UnityEngine;

namespace FruitSimulation.Source.Animators
{
    /// <summary>
    /// Just handles tree animation stuff based on composition.
    /// </summary>
    public class TreeAnimator
    {
        readonly GameFeelVFXConfig _leafsEffectConfig;
        readonly GameFeelVFXConfig _trunkEffectConfig;
        readonly AnimatorService _leafsAnimatorService;
        readonly AnimatorService _trunkAnimatorService;
    
        public TreeAnimator(MonoBehaviour host, GameFeelVFXConfig leafsEffectConfig, GameFeelVFXConfig trunkEffectConfig,
            SpriteRenderer treeSprite, SpriteRenderer trunkSprite)
        {
            _leafsEffectConfig = leafsEffectConfig;
            _trunkEffectConfig = trunkEffectConfig;
            _leafsAnimatorService = new AnimatorService(host, treeSprite);
            _trunkAnimatorService = new AnimatorService(host, trunkSprite);
        }

        public void Tick()
        {
            _leafsAnimatorService.ApplyConstantWobble(_leafsEffectConfig.WobbleFactor,_leafsEffectConfig.WobbleDuration);
            _trunkAnimatorService.ApplyConstantWobble(_trunkEffectConfig.WobbleFactor,_trunkEffectConfig.WobbleDuration);
        }
    
    }
}