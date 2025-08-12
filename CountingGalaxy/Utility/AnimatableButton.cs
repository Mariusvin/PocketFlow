using System;
using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;
using TMPro;

namespace Utility
{
    public class AnimatableButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected bool isInteractive = true;
        [SerializeField] protected float clickCancelThreshPixels = 10f; // if scrolled 10 pixels, do not trigger the click

        [Space(10)][Header("Optional")][Space(10)]
        [Header("Delay settings")]
        [SerializeField] protected float clickDelay = 0f; // delay before the click is triggered
        
        [Header("Scale settings")]
        [SerializeField] protected bool scaleOnPointerDown = true; // scale when clicked
        [SerializeField] protected float scaleDownMultiplier = 0.9f; // scale changes when pressed
        [SerializeField] protected float scaleDuration = 0.15f;
        [SerializeField] private bool usePredefinedScale; // use predefined scale instead of the current scale
        [SerializeField] protected Vector3 predefinedInitScale = Vector3.one;
        [SerializeField] private GameObject scaleTarget; // gameObject to scale. None = this.gameObject
        [SerializeField] protected RectTransform rectTransform;
        
        [Header("Sound settings")]
        [SerializeField] private GenericSoundName pointerDownSound = GenericSoundName.None;
        [SerializeField] private GenericSoundName pointerUpSound = GenericSoundName.None;
        [SerializeField] private GenericSoundName clickSound = GenericSoundName.Click;

        [Header("Particle settings")] 
        [SerializeField] private ParticleSystem pointerDownParticlesOneShot;
        [SerializeField] private float zOffPointerDown;
        [SerializeField] private ParticleSystem pointerUpParticlesOneShot;
        [SerializeField] private float zOffPointerUp;
        [SerializeField] private ParticleSystem clickParticlesOneShot;
        [SerializeField] private float zOffPointerClick;
        
        [Header("Text settings")]
        [SerializeField] private TextMeshPro tmpWorldSpace;
        [SerializeField] private TextMeshProUGUI tmpCanvasSpace;
        
        protected event Action OnClick;

        protected Camera mainCam;
        protected bool isPointerDown;
        
        private Vector3 cachedInitScale;
        private bool isLocked; // used to prevent interaction when disabled
        private Tween scaleTween;
        private float delayTimer;
        
        public Vector2 LastPointerDownScreenPos { get; private set; }
        public Vector3 LastPointerDownWorldPos { get; private set; }
        public Vector2 LastPointerUpScreenPos { get; private set; }
        public Vector3 LastPointerUpWorldPos { get; private set; }
        public Vector3 LastClickWorldPos { get; private set; }
        public Vector2 LastClickScreenPos { get; private set; }
        
        /// <summary>
        /// Returns the last click position if the click is valid, otherwise returns the last pointer down position
        /// </summary>
        public Vector2 LastClickScreenPosValid { get; private set; }
        public Vector3 LastClickWorldPosValid { get; private set; }
        
        public string ButtonText
        {
            get
            {
                if (tmpWorldSpace)
                {
                    return tmpWorldSpace.text;
                }

                if (tmpCanvasSpace)
                {
                    return tmpCanvasSpace.text;
                }

                return string.Empty;
            }
            set
            {
                if (tmpWorldSpace)
                {
                    tmpWorldSpace.text = value;
                }

                if (tmpCanvasSpace)
                {
                    tmpCanvasSpace.text = value;
                }
            }
        }

        protected bool CanClick { get; set; }

        protected bool IsPointerOutOfBounds =>
            InputProvider.TryGetTouchScreenPos(out Vector2 _screenPos) &&
            (LastPointerDownScreenPos - _screenPos).sqrMagnitude > clickCancelThreshPixels * clickCancelThreshPixels;

        public virtual bool IsInteractive
        {
            get => isInteractive;
            set => isInteractive = value;
        }

        protected Vector3 InitScale
        {
            get => usePredefinedScale ? predefinedInitScale : cachedInitScale;
            set
            {
                usePredefinedScale = true;
                predefinedInitScale = value;
                cachedInitScale = value;
            }
        }
        
        protected Camera MainCamera
        {
            get
            {
                if (!mainCam)
                {
                    mainCam = Camera.main;
                }

                return mainCam;
            }
        }

        private GameObject ScaleTarget
        {
            get
            {
                if (!scaleTarget)
                {
                    SetScaleTarget(gameObject);
                }

                return scaleTarget;
            }
            
        }

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (!usePredefinedScale)
            {
                InitScale = transform.localScale;
            }
        }

        protected virtual void OnEnable()
        {
            isLocked = false;
        }
        
        protected virtual void OnDisable()
        {
            isLocked = true;
        }

        protected virtual void Update()
        {
            if(delayTimer > 0f)
            {
                delayTimer -= Time.deltaTime;
                return;
            }
            
            if (!isPointerDown)
            {
                return;
            }

            if (IsPointerOutOfBounds)
            {
                isPointerDown = false;
                CanClick = false;
            }
        }

        protected virtual void OnDestroy()
        {
            ResetCallbacks();
        }

        public void AddTriggerAction(Action _callback)
        {
            OnClick += _callback;
        }

        public void RemoveTriggerAction(Action _action)
        {
            OnClick -= _action;
        }
        
        public void SetScaleTarget(GameObject _target)
        {
            scaleTarget = _target;
            InitScale = _target.transform.localScale;
        }
        
        public bool TryGetWidth(out float _width)
        {
            if (rectTransform)
            {
                _width = rectTransform.rect.width;
                return true;
            }

            _width = 0f;
            return false;
        }

        public virtual void OnPointerClick(PointerEventData _eventData)
        {
            LastClickScreenPos = _eventData.position;
            LastClickWorldPos = _eventData.pointerCurrentRaycast.worldPosition;
            if (!IsInteractive || !CanClick)
            {
                return;
            }

            Click();
        }

        public virtual void OnPointerDown(PointerEventData _eventData)
        {
            if (!IsInteractive)
            {
                return;
            }

            LastPointerDownScreenPos = _eventData.position;
            LastPointerDownWorldPos = _eventData.pointerCurrentRaycast.worldPosition;
            isPointerDown = true;
            CanClick = true;
            if (scaleOnPointerDown)
            {
                ScaleDown(InitScale * scaleDownMultiplier);
            }
            
            PlayPointerDownParticles();
            PlayPointerDownSound();
        }

        public virtual void OnPointerUp(PointerEventData _eventData)
        {
            isPointerDown = false;
            if (!IsInteractive)
            {
                return;
            }
            
            LastPointerUpScreenPos = _eventData.position;
            LastPointerUpWorldPos = _eventData.pointerCurrentRaycast.worldPosition;
            if (scaleOnPointerDown)
            {
                ScaleToInit();
            }
            
            PlayPointerUpParticles();
            PlayPointerUpSound();
        }
        
        public virtual void Show(float _time = 0.2f, float _delay = 0f, Ease _ease = Ease.OutBack, Action _onShowCompleted = null)
        {
            if (_time == 0f)
            {
                CancelScaleTween();
                ScaleTarget.transform.localScale = InitScale;
                _onShowCompleted?.Invoke();
            }
            else
            {
                ScaleAnimated(InitScale, _time, _delay, _ease, 1, CycleMode.Restart, _onShowCompleted);
            }
        }

        public virtual void Hide(float _time = 0.2f, Ease _ease = Ease.OutSine, Action _onHideCompleted = null)
        {
            if (_time == 0f)
            {
                CancelScaleTween();
                ScaleTarget.transform.localScale = Vector3.zero;
                _onHideCompleted?.Invoke();
            }
            else
            {
                ScaleAnimated(Vector3.zero, _time, 0f, _ease, 0, CycleMode.Yoyo, _onHideCompleted);
            }
        }

        public void ResetCallbacks()
        {
            OnClick = null;
        }

        public void Pulsate(float _strength, int _cycles = -1, float _duration = 0.15f)
        {
            if (scaleTween.isAlive)
            {
                return; // If pulsate is in progress, don't start pulsating
            }

            ScaleAnimated(InitScale * _strength, _duration, 0f, Ease.OutSine, _cycles);
        }

        protected virtual void Click()
        {
            if (isLocked)
            {
                return;
            }

            delayTimer = clickDelay;
            LastClickScreenPosValid = LastClickScreenPos;
            LastClickWorldPosValid = LastClickWorldPos;
            PlayClickParticles();
            PlayClickSound();
            OnClick?.Invoke();
        }

        protected virtual void PlayPointerDownSound()
        {
            AudioController.TryPlayGenericSoundByName(pointerDownSound);
        }

        protected virtual void PlayPointerUpSound()
        {
            AudioController.TryPlayGenericSoundByName(pointerUpSound);
        }

        protected virtual void PlayClickSound()
        {
            AudioController.TryPlayGenericSoundByName(clickSound);
        }

        protected virtual void ScaleDown(Vector3 _targetScale, float _time = 0.15f, Ease _ease = Ease.OutSine)
        {
            if (ScaleTarget.transform.localScale == _targetScale)
            {
                return;
            }
            
            CancelScaleTween();
            scaleTween = Tween.Scale(ScaleTarget.transform, _targetScale, _time, _ease);
        }

        protected virtual void ScaleToInit(Ease _ease = Ease.OutBack)
        {
            ScaleAnimated(InitScale, scaleDuration, 0f, _ease);
        }
        
        // ========= Predefined tweens =========
        
        protected void ScaleAnimated(Vector3 _targetScale, float _time = 0.15f, float _delay = 0f, Ease _ease = Ease.OutBack, int _cycles = 0, CycleMode _mode = CycleMode.Yoyo, Action _onScaleCompleted = null)
        {
            CancelScaleTween();
            if(ScaleTarget.transform.localScale == _targetScale)
            {
                _onScaleCompleted?.Invoke();
                return;
            }
            
            if(_onScaleCompleted != null)
            {
                scaleTween = Tween.Scale(ScaleTarget.transform, _targetScale, _time, _ease, _cycles, _mode, _delay).OnComplete(_onScaleCompleted);
            }
            else
            {
                scaleTween = Tween.Scale(ScaleTarget.transform, _targetScale, _time, _ease, _cycles, _mode, _delay);
            }
        }

        protected void CancelScaleTween()
        {
            scaleTween.Stop();
        }
        
        // ======== Particle methods =========
        
        protected virtual void PlayPointerDownParticles()
        {
            if (pointerDownParticlesOneShot)
            {
                Vector3 _target = LastPointerDownWorldPos;
                _target.z += zOffPointerDown;
                ParticlesPlayer.PlayParticlesSimple(pointerDownParticlesOneShot, _target);
            }
        }
        
        protected virtual void PlayPointerUpParticles()
        {
            if (pointerUpParticlesOneShot)
            {
                Vector3 _target = LastPointerUpWorldPos;
                _target.z += zOffPointerUp;
                ParticlesPlayer.PlayParticlesSimple(pointerUpParticlesOneShot, _target);
            }
        }
        
        protected virtual void PlayClickParticles()
        {
            if (clickParticlesOneShot)
            {
                Vector3 _target = LastClickWorldPosValid;
                _target.z += zOffPointerClick;
                ParticlesPlayer.PlayParticlesSimple(clickParticlesOneShot, _target);
            }
        }
    }
}