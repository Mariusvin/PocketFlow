using System;
using UnityEngine.UI;
using UnityEngine;
using PrimeTween;
using TMPro;
using Utility;

namespace Activities.Shared.UI
{
    public class BaseActivityUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool disableProgressBar;
        [SerializeField] private bool disableLevelTexts;

        [Header("Button References")]
        [SerializeField] private AnimatableButton menuButton;
        [SerializeField] private AnimatableButton restartButton;
        [SerializeField] private AnimatableButton nextButton;
        [SerializeField] private AnimatableButton previousButton;

        [Header("Text References")]
        [SerializeField] private GameObject levelObject;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private GameObject levelTitleObject; // In editor only
        [SerializeField] private TextMeshProUGUI levelTitleText; // In editor only

        [Header("Progress Bar References")]
        [SerializeField] private GameObject progressBarObject;
        [SerializeField] private Image progressBarFill;

        private const float PROGRESS_UPDATE_DURATION_SECONDS = 0.5f;
        private const string LEVEL_TEXT_PREFIX = "Level";

        private event Action OnMenuButtonClick;
        private event Action OnRestartButtonClick;
        private event Action OnNextButtonClick;
        private event Action OnPreviousButtonClick;

        private Tween progressBarTween;

        protected virtual bool AreButtonsInteractive
        {
            set
            {
                menuButton.IsInteractive = value;
                restartButton.IsInteractive = value;
                nextButton.IsInteractive = value;
                previousButton.IsInteractive = value;
            }
        }

        protected virtual void OnEnable()
        {
            menuButton.AddTriggerAction(OnMenuButtonClicked);
            restartButton.AddTriggerAction(OnRestartButtonClicked);
            nextButton.AddTriggerAction(OnNextButtonClicked);
            previousButton.AddTriggerAction(OnPreviousButtonClicked);
        }

        protected virtual void OnDisable()
        {
            menuButton.RemoveTriggerAction(OnMenuButtonClicked);
            restartButton.RemoveTriggerAction(OnRestartButtonClicked);
            nextButton.RemoveTriggerAction(OnNextButtonClicked);
            previousButton.RemoveTriggerAction(OnPreviousButtonClicked);
        }

        public virtual void Initialize(int _currentLevel, DifficultyRating _currentDifficultyRating, Action _onMenuButtonClick, Action _onRestartButtonClick, Action _onNextButtonClick, Action _onPreviousButtonClick)
        {
            OnMenuButtonClick = _onMenuButtonClick;
            OnRestartButtonClick = _onRestartButtonClick;
            OnNextButtonClick = _onNextButtonClick;
            OnPreviousButtonClick = _onPreviousButtonClick;

#if !UNITY_EDITOR
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
            levelTitleObject.SetActive(false);
#endif

            if (disableProgressBar)
            {
                progressBarObject.SetActive(false);
            }

            if (disableLevelTexts)
            {
                levelObject.SetActive(false);
                levelTitleObject.SetActive(false);
            }
            else
            {
                levelText.text = $"{LEVEL_TEXT_PREFIX} {_currentLevel}";
                levelTitleText.text = _currentDifficultyRating.ToString();
            }
        }

        public void UpdateProgressBar(float _targetProgress)
        {
            if (disableProgressBar)
            {
                return;
            }

            progressBarTween.Stop();
            progressBarTween = Tween.UIFillAmount(progressBarFill, _targetProgress, PROGRESS_UPDATE_DURATION_SECONDS, Ease.OutSine);
        }

        private void OnMenuButtonClicked()
        {
            AreButtonsInteractive = false;
            OnMenuButtonClick?.Invoke();
        }

        private void OnRestartButtonClicked()
        {
            AreButtonsInteractive = false;
            OnRestartButtonClick?.Invoke();
        }

        private void OnNextButtonClicked()
        {
            AreButtonsInteractive = false;
            OnNextButtonClick?.Invoke();
        }

        private void OnPreviousButtonClicked()
        {
            AreButtonsInteractive = false;
            OnPreviousButtonClick?.Invoke();
        }
    }
}