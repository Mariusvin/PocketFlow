using System;
using UnityEngine.UI;
using UnityEngine;
using PrimeTween;
using TMPro;
using Utility;

namespace Activities.Shared.CornerFluvsie
{
    public class CornerBubbleSlot : AnimatableButton
    {
        [Header("References")]
        [SerializeField] private Image slotImage;
        [SerializeField] private Image checkmarkImage;
        [SerializeField] private TextMeshProUGUI slotText;
        
        public event Action OnActiveStateChange;
        
        private const float SCALE_DURATION_SECONDS = 0.2f;

        private event Action<string> OnSlotClick;

        private Tween scaleTween;
        
        private CornerBubbleSlotData slotData;

        public bool HasData => slotData.ContainsData;

        public bool IsActive
        {
            get => gameObject.activeSelf;
            private set
            {
                if (!value)
                {
                    ScaleSlot(Vector3.zero, _duration: 0f, _easeType: Ease.OutBack);
                }
                
                gameObject.SetActive(value);
                OnActiveStateChange?.Invoke();
            }
        }

        public bool CheckmarkActive
        {
            get => checkmarkImage.gameObject.activeSelf;
            set => checkmarkImage.gameObject.SetActive(value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AddTriggerAction(OnSlotClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RemoveTriggerAction(OnSlotClicked);
        }

        public void UpdateBubbleSlot(CornerBubbleSlotData _data, Action<string> _onSlotClick)
        {
            slotData = _data;
            OnSlotClick = _onSlotClick;
            if (!slotData.ContainsData)
            {
                return;
            }
            
            if (!slotData.SlotSprite)
            {
                slotImage.gameObject.SetActive(false);
            }
            else
            {
                slotImage.gameObject.SetActive(true);
                slotImage.sprite = slotData.SlotSprite;
            }
            
            if(slotData.SlotText.Length <= 0)
            {
                slotText.gameObject.SetActive(false);
            }
            else
            {
                slotText.gameObject.SetActive(true);
                slotText.text = slotData.SlotText;
                slotText.enableAutoSizing = slotData.TextAutoSize;
                slotText.fontSize = slotData.TextSize;
            }
        }

        public void Show(float _duration = SCALE_DURATION_SECONDS, Action _onComplete = null)
        {
            IsActive = true;
            ScaleSlot(Vector3.one, Ease.OutBack, _duration, _onComplete);
        }

        public void Hide(float _duration = SCALE_DURATION_SECONDS, bool _deactivateOnComplete = true, Action _onComplete = null)
        {
            if (_duration == 0f)
            {
                Complete();
                return;
            }
            
            ScaleSlot(Vector3.zero, Ease.OutQuart, _duration, Complete);
            //local method
            void Complete()
            {
                _onComplete?.Invoke();
                if (_deactivateOnComplete)
                {
                    IsActive = false;
                }
            }
        }
        
        public void ResetAnimatedValues()
        {
            transform.localScale = Vector3.zero;
        }

        private void OnSlotClicked()
        {
            OnSlotClick?.Invoke(slotData.SlotText);
        }

        private void ScaleSlot(Vector3 _targetScale, Ease _easeType, float _duration = SCALE_DURATION_SECONDS, Action _onComplete = null)
        {
            scaleTween.Stop();

            if (_duration == 0f)
            {
                transform.localScale = _targetScale;
                _onComplete?.Invoke();
            }
            else
            {
                scaleTween = Tween.Scale(transform, _targetScale, _duration, _easeType);
                
                if (_onComplete != null)
                {
                    scaleTween = scaleTween.OnComplete(_onComplete);
                }
            }
        }
    }
}