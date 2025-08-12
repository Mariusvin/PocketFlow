using UnityEngine;

namespace Utility.Sprites
{
    public class SpriteOverlay : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float transitionTime;
        private float targetAlpha;
        private float startAlpha;
        private float currentAlpha;
        private float currentTime;
        private bool isTransitioning;
        private bool disableOnComplete;
        private Color cachedColor;
        
        private void Update()
        {
            if (!isTransitioning)
            {
                return;
            }
            
            currentTime += Time.deltaTime;
            float _frac = currentTime / transitionTime;
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, _frac);
            cachedColor.a = currentAlpha;
            spriteRenderer.color = cachedColor;

            if (_frac < 1f)
            {
                return;
            }

            isTransitioning = false;
            if (targetAlpha == 0f && disableOnComplete)
            {
                gameObject.SetActive(false);
            }
        }

        public void EnableOverlay(float _alpha, float _duration)
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer is not assigned.");
                return;
            }

            cachedColor = spriteRenderer.color;
            startAlpha = cachedColor.a;
            targetAlpha = _alpha;
            transitionTime = _duration;
            currentTime = 0f;
            isTransitioning = true;
        }
        
        public void DisableOverlay(float _duration, bool _disableOnComplete = true)
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer is not assigned.");
                return;
            }

            cachedColor = spriteRenderer.color;
            startAlpha = cachedColor.a;
            targetAlpha = 0f;
            currentTime = 0f;
            transitionTime = _duration;
            isTransitioning = true;
            disableOnComplete = _disableOnComplete;
        }
    }
}
