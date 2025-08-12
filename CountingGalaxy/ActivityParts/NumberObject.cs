using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using Utility;
using Utility.Sprites;
using Random = UnityEngine.Random;

namespace Activities.CountingGalaxy.ActivityParts
{
    public class NumberObject : AnimatableButton
    {
        [Header("Number Object")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer numberSpriteRenderer;
        [SerializeField] private FloatingObject floatingObject;
        [SerializeField] private ParticleSystem trailParticles;
        [SerializeField] private ParticleSystem numberAppearParticlesPrefab;
        
        public event Action<NumberObject> OnNumberClicked;
        
        private Vector3 numberInitScale;
        private Tween moveTween;
        private Tween scaleTween;
        private Tween numberScaleTween;
        private Tween rotationTween;
        
        public int AssignedNumber { get; private set; }

        public void Setup(int _number, Sprite _objectVisual, Sprite _numberVisual)
        {
            spriteRenderer.sprite = _objectVisual;
            numberSpriteRenderer.sprite = _numberVisual;
            AssignedNumber = _number;
            Transform _numScale = numberSpriteRenderer.transform;
            numberInitScale = _numScale.localScale;
            _numScale.localScale = Vector3.zero;
        }

        public void FollowPath(List<Vector3> _bezierControlPoints, float _duration, Action _onReach)
        {
            if (_bezierControlPoints == null || _bezierControlPoints.Count < 4)
            {
                Debug.LogError("Path control points are not set correctly.");
                return;
            }
            
            trailParticles.Play();
            moveTween.Stop();
            Vector3 _start = _bezierControlPoints[0];
            Vector3 _end = _bezierControlPoints[^1];
            Vector3 _control1 = _bezierControlPoints[1];
            Vector3 _control2 = _bezierControlPoints[2];
            moveTween = Tween.Custom(0f, 1f, _duration, TweenUpdate, Ease.InSine).OnComplete(StopTrailAndInvokeCallback);
            
            // Local methods
            void TweenUpdate(float _t)
            {
                Vector3 _pos = Bezier.EvaluateCubic(_start, _control1, _control2, _end, _t);
                _pos.z = _end.z;
                transform.position = _pos;
            }
            
            void StopTrailAndInvokeCallback()
            {
                trailParticles.Stop();
                _onReach?.Invoke();
            }
        }
        
        public void ScaleTo(float _targetScale, float _duration, Ease _ease)
        {
            scaleTween.Stop();
            scaleTween = Tween.Scale(transform, _targetScale, _duration, _ease);
        }

        public void RotateTo(float _targetZ, float _duration)
        {
            rotationTween.Stop();
            rotationTween = Tween.LocalRotation(transform, Vector3.forward * _targetZ, _duration, Ease.OutSine);
        }

        public void CacheScale()
        {
            SetScaleTarget(gameObject);   
        }
        
        public void EnableFloating()
        {
            if (floatingObject != null)
            {
                floatingObject.Begin();
            }
        }
        
        public void DisableFloating()
        {
            if (floatingObject != null)
            {
                floatingObject.enabled = false;
            }
        }

        public void UpscaleNumber(float _duration)
        {
            Color _color = SpritesComparer.CalculateAverageColor(numberSpriteRenderer.sprite);
            ParticlesPlayer.PlayParticlesSimple(numberAppearParticlesPrefab, numberSpriteRenderer.transform.position, _color);
            numberScaleTween.Stop();
            numberScaleTween = Tween.Scale(numberSpriteRenderer.transform, numberInitScale, _duration, Ease.OutBack);
        }

        protected override void Click()
        {
            base.Click();
            OnNumberClicked?.Invoke(this);
        }
    }
}
