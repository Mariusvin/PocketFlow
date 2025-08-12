using System;
using System.Collections;
using Audio;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utility.UI
{
    public class LoadingScreen : Singleton<LoadingScreen>
    {
        [Header("Visuals")]
        [SerializeField] private Image maskImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image centerImage;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Sprite defaultLoadingSprite;
        [SerializeField] private Color defaultBackgroundColor = Color.white;

        [Header("Audio")]
        [SerializeField] private AudioClip defaultTransitionInSound;
        [SerializeField] private AudioClip defaultTransitionOutSound;
        
        private event Action OnAnimationCompleted; 
        
        private const float MIN_SHOWING_DURATION = 2f;
        // Appearing
        private const float BACKGROUND_CLOSE_DURATION = 0.9f;
        private const float TEXT_REVEAL_DURATION = 0.65f;
        private const float CENTER_IMAGE_REVEAL_DURATION = 0.3f;
        // Closing
        private const float BACKGROUND_OPEN_DURATION = 0.9f;
        private const float TEXT_HIDE_DURATION = 0.3f;
        private const float CENTER_IMAGE_HIDE_DURATION = 0.25f;
        // Misc
        private const float MASK_IMAGE_WIDTH_MAX = 4096f;
        private const string DEFAULT_LOADING_TEXT = "Loading";
        private const string TMP_EFFECTS_PREFIX = "<jump><sketchy>"; 
        private const string TMP_EFFECTS_SUFFIX = "</></>";
        
        private float centerImageWidth;
        private WaitUntil waitUntilWidthThreshold;
        private Coroutine loadingScreenRoutine;
        private RectTransform maskRect;
        private RectTransform centerImageRect;
        
        // Tweens
        private Tween bgValueTween;
        private Tween textValueTween;
        private Tween centerImageValueTween;
        
        private bool IsTMPEffectsInstalled { get; } = true; // Install & setup from https://github.com/Luca3317/TMPEffects
        
        private bool IsLoadingScreenActive { get; set; }
        
        private bool IsAnimationInProgress => loadingScreenRoutine != null;
        
        private bool EnableEverything
        {
            set
            {
                gameObject.SetActive(value);
                maskRect.gameObject.SetActive(value);
                centerImage.gameObject.SetActive(value);
                loadingText.gameObject.SetActive(value);
                IsLoadingScreenActive = value;
            }
        }
        
        protected override void ValidAwake()
        {
            base.ValidAwake();
            
            // Don't destroy on load
            if (transform.parent)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            
            // Caching
            maskRect = maskImage.GetComponent<RectTransform>();
            centerImageRect = centerImage.GetComponent<RectTransform>();
            centerImageWidth = centerImageRect.sizeDelta.x;
            waitUntilWidthThreshold = new WaitUntil(IsImageWidthReached);
            
            // Hide everything
            ResetToInitialValues();
            EnableEverything = false;
        }

        /// <summary>
        /// Show loading screen with default settings
        /// </summary>
        public static bool TryShowLoadingScreen(Action _onMinTimePassed, float _minTime = MIN_SHOWING_DURATION, bool _instantBg = false, string _customLoadingText = "")
        {
            if (!Instance)
            {
                return false;
            }
            
            return Instance.RequestShowLoadingScreen(_onMinTimePassed, _minTime, _instantBg, _customLoadingText);
        }
        
        /// <summary>
        /// Show loading screen with custom settings
        /// </summary>
        public static bool TryShowLoadingScreen(Sprite _centerImage, Color _bgColor, AudioClip _transitionInSound, Action _onMinTimePassed, float _minTime = MIN_SHOWING_DURATION, bool _instantBg = false, string _customLoadingText = "")
        {
            if (!Instance)
            {
                return false;
            }
            
            return Instance.RequestShowLoadingScreen(_centerImage, _bgColor, _transitionInSound, _onMinTimePassed, _minTime, _instantBg, _customLoadingText);
        }
        
        public static bool TryHideLoadingScreen(AudioClip _transitionOutSound = null, Action _onComplete = null)
        {
            if (!Instance)
            {
                return false;
            }
            
            Instance.HideLoadingScreen(_transitionOutSound, _onComplete);
            return true;
        }

        private bool RequestShowLoadingScreen(Action _onMinTimePassed, float _minTime = MIN_SHOWING_DURATION, bool _instantBg = false, string _customLoadingText = "")
        {
            return RequestShowLoadingScreen(defaultLoadingSprite, defaultBackgroundColor, defaultTransitionInSound, _onMinTimePassed, _minTime, _instantBg, _customLoadingText);
        }

        private bool RequestShowLoadingScreen(Sprite _centerImage, Color _bgColor, AudioClip _transitionInSound, Action _onMinTimePassed, float _minTime = MIN_SHOWING_DURATION, bool _instantBg = false, string _customLoadingText = "")
        {
            if(!maskImage || !loadingText)
            {
                Debug.Log("Missing maskImage or loadingText reference in LoadingScreen");
                return false;
            }

            EnableEverything = true;
            CancelCurrentAnimations();
            if (_centerImage)
            {
                maskImage.sprite = _centerImage;
                centerImage.sprite = _centerImage;
            }
            else
            {
                maskImage.sprite = defaultLoadingSprite;
                centerImage.sprite = defaultLoadingSprite;
            }

            backgroundImage.color = _bgColor;
            OnAnimationCompleted = _onMinTimePassed;
            if (!string.IsNullOrEmpty(_customLoadingText))
            {
                loadingText.text = IsTMPEffectsInstalled ? $"{TMP_EFFECTS_PREFIX}{_customLoadingText}{TMP_EFFECTS_SUFFIX}" : _customLoadingText;
            }
            else
            {
                loadingText.text = IsTMPEffectsInstalled ? $"{TMP_EFFECTS_PREFIX}{DEFAULT_LOADING_TEXT}{TMP_EFFECTS_SUFFIX}" : DEFAULT_LOADING_TEXT;
            }
            
            AudioController.TryFadeAllAudioSources();
            AudioController.TryPlaySound(_transitionInSound);
            loadingScreenRoutine = StartCoroutine(ShowLoadingScreenSequence(_minTime, _instantBg));
            return true;
        }
        
        private void HideLoadingScreen(AudioClip _transitionOutSound, Action _onComplete)
        {
            if (!IsLoadingScreenActive)
            {
                _onComplete?.Invoke();
                return; // nothing to hide :-)
            }
            
            CancelCurrentAnimations();
            OnAnimationCompleted = _onComplete;
            
            AudioClip _transitionSound = _transitionOutSound ? _transitionOutSound : defaultTransitionOutSound;
            AudioController.TryPlaySound(_transitionSound);
            loadingScreenRoutine = StartCoroutine(HideLoadingScreenSequence());
        }
        
        private void CancelCurrentAnimations()
        {
            if (!IsAnimationInProgress)
            {
                return;
            }
            
            StopCoroutine(loadingScreenRoutine);
            bgValueTween.Stop();
            textValueTween.Stop();
            centerImageValueTween.Stop();
        }
        
        private void ResetToInitialValues()
        {
            maskRect.sizeDelta = new Vector2(MASK_IMAGE_WIDTH_MAX, maskRect.sizeDelta.y);
            centerImageRect.localScale = Vector3.zero;
            loadingText.transform.localScale = Vector3.zero;
        }
        
#region SHOWING_SEQUENCE
        
        private IEnumerator ShowLoadingScreenSequence(float _minTime, bool _instantBg)
        {
            float _curTime = Time.time;
            RevealBackground(_instantBg);
            yield return waitUntilWidthThreshold;
            
            RevealText();
            RevealCenterImage();
            yield return Yielder.Wait(CENTER_IMAGE_REVEAL_DURATION);
            
            // Wait for minimum duration
            float _passedTime = Time.time - _curTime;
            if (_passedTime < _minTime)
            {
                yield return Yielder.Wait(_minTime - _passedTime);
            }
            
            OnAnimationCompleted?.Invoke();
            OnAnimationCompleted = null;
            loadingScreenRoutine = null;
        }
        
        private void RevealBackground(bool _instant)
        {
            if (_instant)
            {
                maskRect.sizeDelta = new Vector2(0f, maskRect.sizeDelta.y);
                return;
            }

            bgValueTween = Tween.Custom(0f, 1f, BACKGROUND_CLOSE_DURATION, UpdateWidth, Ease.InSine);
            
            // Local method
            void UpdateWidth(float _frac)
            {
                maskRect.sizeDelta = new Vector2(Mathf.Lerp(MASK_IMAGE_WIDTH_MAX, 0f, _frac), maskRect.sizeDelta.y);
            }
        }
        
        private void RevealText()
        {
            textValueTween = Tween.Scale(loadingText.transform, 1f, TEXT_REVEAL_DURATION, Ease.OutBack);
        }
        
        private void RevealCenterImage()
        {
            centerImageValueTween = Tween.Scale(centerImage.transform, 1f, CENTER_IMAGE_REVEAL_DURATION, Ease.OutBounce);
        }

        private bool IsImageWidthReached()
        {
            return maskRect.sizeDelta.x <= centerImageWidth;
        }
        
#endregion

#region HIDING SEQUENCE
        
        private IEnumerator HideLoadingScreenSequence()
        {
            HideBackground();
            HideText();
            HideCenterImage();
            yield return Yielder.Wait(BACKGROUND_OPEN_DURATION);
            
            OnAnimationCompleted?.Invoke();
            OnAnimationCompleted = null;
            loadingScreenRoutine = null;
            EnableEverything = false;
            ResetToInitialValues();
        }

        private void HideBackground()
        {
            bgValueTween = Tween.Custom(0f, 1f, BACKGROUND_OPEN_DURATION, UpdateWidth, Ease.InSine);
            
            // Local method
            void UpdateWidth(float _frac)
            {
                maskRect.sizeDelta = new Vector2(Mathf.Lerp(0f, MASK_IMAGE_WIDTH_MAX, _frac), maskRect.sizeDelta.y);
            }
        }
        
        private void HideText()
        {
            textValueTween = Tween.Scale(loadingText.transform, 0f, TEXT_HIDE_DURATION, Ease.InBack);
        }
        
        private void HideCenterImage()
        {
            centerImageValueTween = Tween.Scale(centerImage.transform, 0f, CENTER_IMAGE_HIDE_DURATION, Ease.InBack);
        }
        
#endregion
    }
}
