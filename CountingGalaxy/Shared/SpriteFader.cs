using System;
using UnityEngine;
using PrimeTween;

namespace Activities.Shared
{
    public class SpriteFader : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fadeDurationSeconds;

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Tween alphaTween;

        public Sprite Sprite => spriteRenderer.sprite;

        public void FadeInSprite(float _targetAlpha = 1.0f, Ease _fadeEaseType = Ease.OutQuad, Action _onFadeCompleted = null)
        {
            FadeSprite(_targetAlpha, _fadeEaseType, _onFadeCompleted);
        }

        public void FadeOutSprite(float _targetAlpha = 0.0f, Ease _fadeEaseType = Ease.OutQuart, Action _onFadeCompleted = null)
        {
            FadeSprite(_targetAlpha, _fadeEaseType, _onFadeCompleted);
        }

        private void FadeSprite(float _targetAlpha, Ease _fadeEaseType, Action _onFadeCompleted)
        {
            Color _color = spriteRenderer.color;
            float _startAlpha = spriteRenderer.color.a;

            alphaTween.Stop();
            
            alphaTween = Tween.Custom(
                startValue: _startAlpha,
                endValue: _targetAlpha,
                duration: fadeDurationSeconds,
                onValueChange: _newAlphaValue =>
                {
                    _color.a = _newAlphaValue;
                    spriteRenderer.color = _color;
                },
                ease: _fadeEaseType
            );
            
            if (_onFadeCompleted != null)
            {
                alphaTween = alphaTween.OnComplete(_onFadeCompleted);
            }
        }
    }
}