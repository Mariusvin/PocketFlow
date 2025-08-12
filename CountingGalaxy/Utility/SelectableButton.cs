using System;
using PrimeTween;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Utility
{
    public class SelectableButton : AnimatableButton
    {
        [Header("Selectable Button Settings")]
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private Color selectedFillColor;
        [SerializeField] private Color deselectedTextColor = Color.black;
        [SerializeField] private Color deselectedFillColor = Color.white;
        
        [Header("Selectable Button References")] 
        [SerializeField] private bool disableFillOnDeselect = true;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image buttonFill;
        [Tooltip("Can be null")][SerializeField] private Sprite selectedFillSprite;

        private const float ROTATE_Z_ANGLE = 6.0f;
        private const float SHAKE_DURATION_SECONDS = 0.15f;

        private event Action<SelectableButton> OnSelectableButtonClicked;

        private Sprite initFillSprite;
        private bool isSelected;
        private Tween rotateTween;

        public string ButtonText
        {
            get => buttonText.text;
            protected set => buttonText.text = value;
        }
        
        public bool IsSelected
        {
            get => isSelected;

            set
            {
                isSelected = value;
                UpdateSelectionVisuals();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            CacheInitValues();
        }

        public void Initialize(Action<SelectableButton> _onSelectableButtonClicked)
        {
            OnSelectableButtonClicked = _onSelectableButtonClicked;
            CacheInitValues();
        }

        public void Initialize(string _text, bool _autoSizeText = true, Action<SelectableButton> _onSelectableButtonClicked = null)
        {
            OnSelectableButtonClicked = _onSelectableButtonClicked;
            buttonText.text = _text;
            buttonText.enableAutoSizing = _autoSizeText;
            CacheInitValues();
        }
        
        public void AddSelectedCallback(Action<SelectableButton> _callback)
        {
            OnSelectableButtonClicked += _callback;
        }

        public void ShakeButton()
        {
            if (rotateTween.isAlive)
            {
                return;
            }

            rotateTween.Stop();
            rotateTween = Tween.PunchLocalRotation(
                transform, 
                new Vector3(0, 0, ROTATE_Z_ANGLE), 
                SHAKE_DURATION_SECONDS,
                10,
                false,
                Ease.OutQuart
            );
        }

        protected override void Click()
        {
            IsSelected = !isSelected;
            OnSelectableButtonClicked?.Invoke(this);
            base.Click();
        }
        
        private void CacheInitValues()
        {
            if (!buttonFill)
            {
                return;
            }

            if (buttonFill.sprite)
            {
                initFillSprite = buttonFill.sprite;
            }
        }

        private void UpdateSelectionVisuals()
        {
            if (buttonText)
            {
                buttonText.color = isSelected ? selectedTextColor : deselectedTextColor;
            }

            if (!buttonFill)
            {
                return;
            }

            if(disableFillOnDeselect)
            {
                buttonFill.enabled = isSelected;
                return;
            }
                
            if (selectedFillSprite)
            {
                buttonFill.sprite = isSelected ? selectedFillSprite : initFillSprite;
            }
                
            buttonFill.color = isSelected ? selectedFillColor : deselectedFillColor;
        }
    }
}