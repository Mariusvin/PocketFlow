using System;
using PrimeTween;
using UnityEngine;

namespace Activities.Shared.Architecture
{
    // Base class for activity parts
    // T - type of the part name enum, e.g. TeethCleaningPartName
    public class ActivityPartBase<T> : MonoBehaviour where T : Enum
    {
        [Header("Base")]
        [SerializeField] protected GameObject partObject;
        [SerializeField] protected float additionalCameraZoom;
        [SerializeField] private T partType;
        [SerializeField] private bool resetCameraZoomOnComplete = true;
        [SerializeField] private bool disablePartObjectOnComplete = true;
        
        private event Action<ActivityPartBase<T>> OnCompleted;

        private const float ZOOM_TOLERANCE = 0.1f;
        private const float ZOOM_DURATION = 0.34f;

        private static Tween staticZoomTween; // Each activity base controls the same camera, so we can use static tween

        protected int mistakesCount; // Todo: use it in the analytics tracking
        private Camera mainCam;
        private bool initCamZoomSet;
        private float initCamZoom;
        
        public bool ResetCameraZoomOnComplete
        {
            get => resetCameraZoomOnComplete;
            set => resetCameraZoomOnComplete = value;
        }

        public bool PartObjectEnabled
        {
            get => partObject && partObject.activeSelf;
            set
            {
                if (partObject)
                {
                    partObject.SetActive(value);
                }
            }
        }

        public bool DisablePartObjectOnComplete
        {
            get => disablePartObjectOnComplete;
            set => disablePartObjectOnComplete = value;
        }
        
        public T PartType => partType;
        
        public int MistakesCount => mistakesCount;

        protected Camera MainCam
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

        protected bool InProgress { get; private set; }
        
        private float InitCamZoom
        {
            get
            {
                if (!initCamZoomSet)
                {
                    initCamZoom = MainCam.orthographicSize;
                    initCamZoomSet = true;
                }
                return initCamZoom;
            }
        }
        
        public virtual void Initialize()
        {
            mainCam = Camera.main;
        }

        public virtual void Begin(Action<ActivityPartBase<T>> _onPartCompleted)
        {
            OnCompleted = _onPartCompleted;
            InProgress = true;
            PartObjectEnabled = true;
            ZoomCamera();
        }

        protected virtual void Complete()
        {
            InProgress = false;
            if (disablePartObjectOnComplete)
            {
                PartObjectEnabled = false;
            }
            
            if (resetCameraZoomOnComplete)
            { 
                ResetCameraZoom();
            }
            
            OnCompleted?.Invoke(this);
        }

        protected virtual void ZoomCamera()
        {
            AnimateCameraSize(InitCamZoom - additionalCameraZoom, Ease.InSine);
        }

        protected virtual void ResetCameraZoom()
        {
            AnimateCameraSize(InitCamZoom, Ease.OutSine);
        }

        private void AnimateCameraSize(float _target, Ease _easing)
        {
            if (Math.Abs(MainCam.orthographicSize - _target) < ZOOM_TOLERANCE)
            {
                return;
            }
            
            staticZoomTween.Stop();
            staticZoomTween = Tween.CameraOrthographicSize(MainCam, _target, ZOOM_DURATION, _easing);
        }
    }
} 