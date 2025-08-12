using System;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace Utility
{
    public class BackgroundOverlay : AnimatableButton
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Color targetColor;

        private Color initColor;
        private Color chosenColor;
        private bool isEnabled;
        private Tween colorTween;
        
        public event Action OnOverlayClicked;

        public bool IsRaycastTarget
        {
            set => targetImage.raycastTarget = value;
        }

        protected override void Awake()
        {
            base.Awake();
            chosenColor = targetColor;
            initColor = targetImage.color;
        }

        protected override void Click()
        {
            base.Click();
            OnOverlayClicked?.Invoke();
        }

        public void EnableOverlay(float _fadeLength = 0.45f)
        {
            if (isEnabled)
            {
                return;
            }

            chosenColor = targetColor;
            SetOverlay(_fadeLength, true);
        }
        
        public void EnableOverlay(float _fadeLength, Color _color)
        {
            if (isEnabled)
            {
                return;
            }
            
            chosenColor = _color;
            SetOverlay(_fadeLength, true);
        }

        public void DisableOverlay(float _fadeLength = 0.45f)
        {
            if (!isEnabled)
            {
                return;
            }

            chosenColor = initColor;
            SetOverlay(_fadeLength, false);
        }

        private void SetOverlay(float _fadeLength, bool _enabled)
        { 
            colorTween.Stop();
            IsRaycastTarget = _enabled;
            isEnabled = _enabled;
            
            if (_fadeLength == 0f)
            {
                targetImage.color = chosenColor;
            }
            else
            {
                colorTween = Tween.Color(targetImage, chosenColor, _fadeLength, Ease.OutSine);
            }
        }
    }
}