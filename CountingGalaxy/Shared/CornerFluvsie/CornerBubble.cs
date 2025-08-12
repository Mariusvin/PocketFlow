using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;
using PrimeTween;
using Utility;

namespace Activities.Shared.CornerFluvsie
{
    public class CornerBubble : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<CornerBubbleSlot> slots;
        [SerializeField] private GridLayoutGroup layoutGroup;
        [SerializeField] private AnimatableButton questionMark;

        private const float SCALE_DURATION_SECONDS = 0.25f;
        private const float FAST_DELAY_DURATION_SECONDS = 0.15f;
        private const float SLOW_DELAY_DURATION_SECONDS = 0.65f;
        private const float SLOTS_HINT_DURATION_SECONDS = 1.5f;
        private const float PULSATE_STRENGTH = 1.07f;
        private const float PULSATE_DURATION = 0.15f;
        private const float MAX_SLOT_HEIGHT = 150.0f;
        private const int PULSATE_COUNT = 3;

        private bool areSlotsHinted;
        private float slotsRevealDelayDuration;
        private float timeElapsed;
        private int nextSlotIndex;
        private Tween scaleTween;
        private Tween sizeTween;
        private Coroutine animateSlotsCoroutine;
        private RectTransform layoutGroupRectTransform;

        private Vector3 CellSize
        {
            set => layoutGroup.cellSize = value;
        }
        
        private int ActiveSlotsCount => slots.Count(_slot => _slot.IsActive);
        
        private Rect LayoutGroupRect => layoutGroupRectTransform ? layoutGroupRectTransform.rect : (layoutGroupRectTransform = layoutGroup.GetComponent<RectTransform>()).rect;

        private void OnEnable()
        {
            foreach (CornerBubbleSlot _slot in slots)
            {
                _slot.OnActiveStateChange += ResizeCells;
            }
            questionMark.AddTriggerAction(OnHintButtonClick);
        }

        private void OnDisable()
        {
            foreach (CornerBubbleSlot _slot in slots)
            {
                _slot.OnActiveStateChange -= ResizeCells;
            }
            
            questionMark.RemoveTriggerAction(OnHintButtonClick);
        }

        private void Update()
        {
            if (!areSlotsHinted)
            {
                return;
            }

            if (timeElapsed < SLOTS_HINT_DURATION_SECONDS)
            {
                timeElapsed += Time.deltaTime;
                return;
            }
            
            timeElapsed = 0.0f;
            areSlotsHinted = false;
            HideSlotsAnimated(false, ShowQuestionMark);
        }
        
        // Updates the data (sprites / text etc.). Does not reveal the slots
        public void UpdateSlotsData(List<CornerBubbleSlotData> _slotsData, Action<string> _onSlotClick = null)
        {
            if (_slotsData == null)
            {
                Debug.LogError("Slots data is null. Hiding the bubble");
                ScaleBubble(Vector3.zero, Ease.OutQuart);
                return;
            }
            
            UpdateBubbleSlotsData(_slotsData, _onSlotClick);
        }
        
        public void SetCheckmark(int _slotIndex, bool _isActive)
        {
            if (slots.Count <= _slotIndex)
            {
                Debug.LogError($"Failed to {(_isActive ? "apply" : "remove")} checkmark. Slot index '{_slotIndex}' is out of range.");
                return;
            }

            slots[_slotIndex].CheckmarkActive = _isActive;
        }

        public void ResetCheckmarks()
        {
            foreach (CornerBubbleSlot _slot in slots)
            {
                _slot.CheckmarkActive = false;
            }
        }

        public void Show()
        {
            ScaleBubble(Vector3.one, Ease.OutBack);
        }

        public void Hide()
        {
            ScaleBubble(Vector3.zero, Ease.OutQuart);
        }

        public void Pulsate()
        {
            // Cancel any existing scale tween
            scaleTween.Stop();
            
            // Create a pulsating scale tween
            scaleTween = Tween.Scale(
                target: transform,
                endValue: Vector3.one * PULSATE_STRENGTH,
                duration: PULSATE_DURATION,
                ease: Ease.InOutSine,
                cycles: PULSATE_COUNT,
                cycleMode: CycleMode.Yoyo
            );
        }

        public void HideSlotsAnimated(bool _deactivateSlots = true, Action _onComplete = null)
        {
            if (animateSlotsCoroutine != null)
            {
                StopCoroutine(animateSlotsCoroutine);
            }

            animateSlotsCoroutine = StartCoroutine(HideSlotsCoroutine(_deactivateSlots, _onComplete));
        }

        public void HideSlotsImmediately()
        {
            foreach (CornerBubbleSlot _slot in slots)
            {
                if (!_slot || !_slot.IsActive)
                {
                    continue;
                }

                _slot.Hide(0f);
            }
        }

        public void ShowQuestionMark()
        {
            questionMark.Show(_ease: Ease.OutQuad);
        }

        public void HideQuestionMark()
        {
            questionMark.Hide(_ease: Ease.OutQuad);
        }

        public void ShowSlotAtIndex(int _slotIndex)
        {
            nextSlotIndex = _slotIndex;
            TryShowingNextSlot();
        }

        public void ShowAllSlots(bool _fastReveal = true, Action _onComplete = null)
        {
            nextSlotIndex = 0;
            slotsRevealDelayDuration = _fastReveal ? FAST_DELAY_DURATION_SECONDS : SLOW_DELAY_DURATION_SECONDS;
            HideSlotsImmediately();
            ShowAllSlotsAnimated(_onComplete);
        }
        
        public bool TryShowingNextSlot(bool _hidePrevious = false)
        {
            if (nextSlotIndex >= slots.Count)
            {
                Debug.LogError($"Out of range! {nextSlotIndex} >= {slots.Count}");
                return false;
            }
            
            if (_hidePrevious && nextSlotIndex > 0)
            {
                slots[nextSlotIndex - 1].Hide(0f);
            }

            CornerBubbleSlot _slot = slots[nextSlotIndex++];
            if (!_slot || _slot.IsActive || !_slot.HasData)
            {
                return false;
            }

            _slot.Show();
            return true;
        }

        public void ResetAnimatedValues()
        {
            transform.localScale = Vector3.zero;
            questionMark.Hide(0f);
            
            foreach (CornerBubbleSlot _slot in slots)
            {
                if (!_slot)
                {
                    continue;
                }

                _slot.ResetAnimatedValues();
            }
        }

        // Updates the slots with the given data. Does not reveal them.
        private void UpdateBubbleSlotsData(IReadOnlyList<CornerBubbleSlotData> _slotsData, Action<string> _onSlotClick)
        {
            if (_slotsData.Count > slots.Count)
            {
                Debug.LogError($"Slots data count is greater than slots count. The last '{_slotsData.Count - slots.Count}' data will be ignored!");
            }

            for (int i = 0; i < slots.Count; i++)
            {
                CornerBubbleSlot _slot = slots[i]; // turn off every slot
                _slot.Hide(0f);
                
                if (i >= _slotsData.Count)
                {
                    slots[i].UpdateBubbleSlot(new CornerBubbleSlotData(), _onSlotClick);
                }
                else
                {
                    _slot.UpdateBubbleSlot(_slotsData[i], _onSlotClick);
                }
            }
        }

        private void ResizeCells()
        {
            if(ActiveSlotsCount <= 0)
            {
                return;
            }

            int _rowCount = ActiveSlotsCount > 2 ? 2 : 1;
            layoutGroup.constraintCount = _rowCount;
            
            Vector2 _currentCellSize = layoutGroup.cellSize;
            float _totalSpacing = layoutGroup.spacing.x;
            float _cellSizeX = ((LayoutGroupRect.width - LayoutGroupRect.xMin - LayoutGroupRect.xMax) / Mathf.Min(_rowCount, ActiveSlotsCount)) / 2;
            float _cellSizeY = Mathf.Min(MAX_SLOT_HEIGHT, LayoutGroupRect.height / _rowCount);
            Vector3 _targetCellSize = new(_cellSizeX, _cellSizeY);
            
            // Cancel previous tween
            sizeTween.Stop();
            
            // Create new custom tween for cell size
            sizeTween = Tween.Custom(
                startValue: 0f,
                endValue: 1f,
                duration: 0.15f,
                onValueChange: ResizeCell,
                ease: Ease.OutCubic
            );
            // Local method
            void ResizeCell(float _frac)
            {
                CellSize = Vector3.Lerp(_currentCellSize, _targetCellSize, _frac);
            }
        }

        private void ShowSlots()
        {
            foreach (CornerBubbleSlot _slot in slots)
            {
                if (!_slot || !_slot.IsActive)
                {
                    continue;
                }

                _slot.Show();
            }
        }

        private void ShowAllSlotsAnimated(Action _onComplete)
        {
            if (animateSlotsCoroutine != null)
            {
                StopCoroutine(animateSlotsCoroutine);
            }

            animateSlotsCoroutine = StartCoroutine(ShowSlotsCoroutine(_onComplete));
        }

        private IEnumerator ShowSlotsCoroutine(Action _onComplete)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (TryShowingNextSlot())
                {
                    yield return Yielder.Wait(slotsRevealDelayDuration);
                }
            }
            
            yield return Yielder.Wait(slotsRevealDelayDuration);
            _onComplete?.Invoke();
            animateSlotsCoroutine = null;
        }

        private IEnumerator HideSlotsCoroutine(bool _deactivateSlots, Action _onComplete)
        {
            for (int _index = slots.Count - 1; _index >= 0; _index--) // Hide in reversed order
            {
                CornerBubbleSlot _slot = slots[_index];
                if (!_slot || !_slot.IsActive)
                {
                    continue;
                }

                _slot.Hide(_deactivateOnComplete: _deactivateSlots);
                nextSlotIndex = _index;
                yield return Yielder.Wait(FAST_DELAY_DURATION_SECONDS);
            }

            _onComplete?.Invoke();
            animateSlotsCoroutine = null;
        }

        private void OnHintButtonClick()
        {
            areSlotsHinted = true;
            ShowSlots();
            HideQuestionMark();
        }

        private void ScaleBubble(Vector3 _targetScale, Ease _easeType)
        {
            // Cancel previous scale tween
            scaleTween.Stop();
            
            // Create new scale tween
            scaleTween = Tween.Scale(transform, _targetScale, SCALE_DURATION_SECONDS, _easeType);
        }
    }
}