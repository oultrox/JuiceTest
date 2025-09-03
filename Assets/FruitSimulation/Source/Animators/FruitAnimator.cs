using AnimationService;
using FruitSimulation.Source.Configs;
using UnityEngine;

namespace FruitSimulation.Source.Animators
{
    /// <summary>
    /// Just animates fruits!
    /// </summary>
    public class FruitAnimator
    {
        readonly AnimatorService _animator;
        readonly GameFeelVFXConfig _juiceConfig;

        public FruitAnimator(MonoBehaviour host, SpriteRenderer spriteRenderer, GameFeelVFXConfig juiceConfig)
        {
            _animator = new AnimatorService(host, spriteRenderer);
            _juiceConfig = juiceConfig;

            _animator.PlayShrinkFromSmallToBig(_juiceConfig.ShrinkFactor, _juiceConfig.ShrinkDuration);
        }

        public void PlayStretch() => _animator.PlayStretch(_juiceConfig.StretchFactor);
        public void PlaySquash() => _animator.PlaySquash(_juiceConfig.SquashFactor);
        public void PlayShrink() => _animator.PlayShrinkFromSmallToBig(_juiceConfig.ShrinkFactor, _juiceConfig.ShrinkDuration);
    }
}