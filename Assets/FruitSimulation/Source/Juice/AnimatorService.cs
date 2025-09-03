using System.Collections;
using UnityEngine;

namespace TestEngine.Source.Juice
{
    /// <summary>
    /// Component for displaying animations and applying code-driven animations.
    /// </summary>
    public class AnimatorService
    {
        readonly MonoBehaviour _coroutineHost;
        SpriteRenderer _spriteRenderer;
        Vector3 _originalScale;
        Quaternion _originalRotation;

        public AnimatorService(MonoBehaviour coroutineHost, SpriteRenderer spriteRenderer)
        {
            _coroutineHost = coroutineHost;
            _spriteRenderer = spriteRenderer;
            _originalScale = _spriteRenderer.transform.localScale;
            _originalRotation = _spriteRenderer.transform.localRotation;
        }

        /// <summary>
        /// Updates rendering flip side.
        /// </summary>
        public void UpdateFacingDirection(float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) > 0.01f)
                _spriteRenderer.flipX = horizontalInput < 0;
        }
        
        /// <summary>
        /// Plays a stretch effect by vertically stretching the scale of the sprite and resetting it.
        /// </summary>
        public void PlayShrinkFromSmallToBig(float scale = 0.2f, float duration = 0.5f)
        {
            Vector3 targetScale = new Vector3(_originalScale.x - scale, _originalScale.y - scale, _originalScale.z);
            _coroutineHost.StartCoroutine(DoScale(targetScale, duration));
        }

        /// <summary>
        /// Plays a stretch effect by vertically stretching the scale of the sprite and resetting it.
        /// </summary>
        public void PlayStretch(float intensity = 0.2f, float duration = 0.2f)
        {
            Vector3 targetScale = new Vector3(_originalScale.x - intensity, _originalScale.y + intensity, _originalScale.z);
            _coroutineHost.StartCoroutine(DoScale(targetScale, duration));
        }

        public void PlaySquash(float intensity = 0.2f, float duration = 0.2f)
        {
            Vector3 targetScale = new Vector3(_originalScale.x + intensity, _originalScale.y - intensity, _originalScale.z);
            _coroutineHost.StartCoroutine(DoScale(targetScale, duration));
        }

        /// <summary>
        /// Continuous effect: Applying a run wobble by lerping the sprite quaternion.
        /// </summary>
        public void ApplyConstantWobble(float intensity, float duration = 5f)
        {
            if (Mathf.Abs(intensity) < 0.01f)
                return;

            float wobble = Mathf.Sin(Time.time * duration) * 0.05f * Mathf.Abs(intensity);
            Quaternion targetRotation = Quaternion.Euler(0, 0, wobble * 10f);
            _spriteRenderer.transform.localRotation = Quaternion.Lerp(
                _spriteRenderer.transform.localRotation, 
                targetRotation, 
                Time.deltaTime * 8f
            );
        }

        /// <summary>
        /// Continuous effect: Resetting smoothly to the initial render transform local size and rotation.
        /// </summary>
        public void SmoothResetSpriteRender()
        {
            _spriteRenderer.transform.localScale = Vector3.Lerp(
                _spriteRenderer.transform.localScale, 
                _originalScale, 
                Time.deltaTime * 5f
            );
            _spriteRenderer.transform.localRotation = Quaternion.Lerp(
                _spriteRenderer.transform.localRotation, 
                _originalRotation, 
                Time.deltaTime * 5f
            );
        }

        IEnumerator DoScale(Vector3 targetScale, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                _spriteRenderer.transform.localScale = Vector3.Lerp(targetScale, _originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.transform.localScale = _originalScale;
        }
    }
}
