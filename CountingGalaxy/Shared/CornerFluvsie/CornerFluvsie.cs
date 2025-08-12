using System.Collections.Generic;
using System;
using UnityEngine;
using PrimeTween;
using SpineAnimations.Fluvsie;

namespace Activities.Shared.CornerFluvsie
{
    public class CornerFluvsie : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool isAnchored;

        [Header("References")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Transform background;
        [SerializeField] private FluvsieUISpineController spineController;
        [SerializeField] private CornerBubble mindBubble;

        private const float DEFAULT_DURATION_SECONDS = 0.5f;
        private const float DURATION_MULTIPLIER = 1.5f;

        private Vector3 initialPosition;
        private Vector3 initialFluvsieScale;
        
        // Store tweens to be able to stop them
        private Tween backgroundScaleTween;
        private Tween fluvsieScaleTween;

        public CornerBubble MindBubble => mindBubble;
        
        /// <summary>
        /// Must be called before any other function
        /// </summary>
        public void Initialize(List<CornerBubbleSlotData> _slotsData, bool _revealAllSlots, Action<string> _onSlotClick = null)
        {
            initialPosition = transform.localPosition;
            initialFluvsieScale = spineController.transform.localScale;
            AnchorTransform(isAnchored);
            spineController.Initialize();
            ResetAnimatedValues();
            mindBubble.UpdateSlotsData(_slotsData, _onSlotClick);
            
            if(_revealAllSlots)
            {
                UpdateShowingSlots(_slotsData, false);
            }
        }

        public void Show(Action _onFluvsieShown = null)
        {
            // Show background
            ScaleTransform(background, Vector3.one, DEFAULT_DURATION_SECONDS, Ease.OutQuart);
            // Show fluvsie. After fluvsie is shown, show mind bubble
            ScaleTransform(spineController.transform, initialFluvsieScale, DEFAULT_DURATION_SECONDS * DURATION_MULTIPLIER, Ease.OutBack, OnFluvsieShown);
            // Fluvsie should start with looping idle animation
            spineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.IdleStandingFrontal, true);
            // Greet the player
            spineController.PlayRandomAnimationOneShot(FluvsieSpineAnimationsMixType.Greetings);

            // Local method
            void OnFluvsieShown()
            {
                _onFluvsieShown?.Invoke();
                mindBubble.Show();
            }
        }
        
        public void UpdateShowingSlots(List<CornerBubbleSlotData> _newSlotsData, bool _animate)
        {
            if (_animate)
            {
                mindBubble.HideSlotsAnimated(_onComplete: RevealNewSlots);
            }
            else
            {
                mindBubble.HideSlotsImmediately();
                RevealNewSlots();
            }

            // Local method
            void RevealNewSlots()
            {
                mindBubble.UpdateSlotsData(_newSlotsData);
                mindBubble.ShowAllSlots();
                
                if (_animate)
                {
                    mindBubble.Pulsate();
                }
            }
        }
        
        public void ResetAllCheckmarks()
        {
            mindBubble.ResetCheckmarks();
        }

        public void PlayHappyReaction()
        {
            spineController.PlayRandomAnimationOneShot(FluvsieSpineAnimationsMixType.HappyMix);
        }

        public void PlayApproveReaction()
        {
            spineController.PlayRandomAnimationOneShot(FluvsieSpineAnimationsMixType.GoodChoice);
        }

        public void PlayDisapproveReaction()
        {
            spineController.PlayRandomAnimationOneShot(FluvsieSpineAnimationsMixType.BadChoice);
        }

        private void ScaleTransform(Transform _transform, Vector3 _targetScale, float _scaleDurationSeconds, Ease _easeType, Action _onScaleCompleted = null)
        {
            // Stop previous tweens on this transform
            if (_transform == background)
            {
                backgroundScaleTween.Stop();
            }
            else if (_transform == spineController.transform)
            {
                fluvsieScaleTween.Stop();
            }
            
            // Create new tween with PrimeTween
            Tween newTween = Tween.Scale(_transform, _targetScale, _scaleDurationSeconds, _easeType);
            
            // Add completion callback if provided
            if (_onScaleCompleted != null)
            {
                newTween = newTween.OnComplete(_onScaleCompleted);
            }
            
            // Store the tween for later cancellation
            if (_transform == background)
            {
                backgroundScaleTween = newTween;
            }
            else if (_transform == spineController.transform)
            {
                fluvsieScaleTween = newTween;
            }
        }

        private void AnchorTransform(bool _isAnchored)
        {
            isAnchored = _isAnchored;
            if (_isAnchored)
            {
                // Anchor corner fluvsie to bottom right corner
                rectTransform.anchorMin = Vector2.right;
                rectTransform.anchorMax = Vector2.right;
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                // Reset corner fluvsie to initial position
                transform.localPosition = initialPosition;
            }
        }

        private void ResetAnimatedValues()
        {
            background.localScale = Vector3.zero;
            spineController.transform.localScale = Vector3.zero;
            mindBubble.ResetAnimatedValues();
        }
    }
}