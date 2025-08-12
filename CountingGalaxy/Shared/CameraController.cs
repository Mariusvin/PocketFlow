using System;
using PrimeTween;
using TutoTOONS.Subscription.Popups;
using UnityEngine;
using Utility;

namespace Activities.Shared
{
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool enableCameraDragging;
        [SerializeField] private float zoomTargetAmount;
        [SerializeField] private float zoomDurationSeconds;
        [SerializeField] private bool verticalScrollEnabled = true;
        [SerializeField] private bool horizontalScrollEnabled = true;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Optional Settings")]
        [SerializeField] private bool enableSmoothDamping;

        [Header("Optional References")]
        [SerializeField] private BoxCollider2D cameraConfiner;

        private const float DRAGGING_SMOOTH_TIME = 0.1f;
        private const float RELEASE_SMOOTH_TIME = 0.75f;
        private const float DISTANCE_ERROR = 0.01f;
        private const float VEL_ERROR_SQR = 0.04f;

        private SubscriptionPopup currentOpenedPopup;
        private Camera mainCamera;
        private Vector3 touchPosition;
        private Vector3 initialPosition;
        private Vector3 currentTouchScreenPosition;
        private Vector3 velocity;
        private bool isDragging;
        private Tween moveTween;
        private Tween zoomTween;

        public bool IsScrollingEnabled
        {
            get => enableCameraDragging && !currentOpenedPopup;
            set => enableCameraDragging = value;
        }

        public bool IsScrolling { get; private set; }

        private Camera MainCamera
        {
            get
            {
                if (!mainCamera)
                {
                    mainCamera = Camera.main;
                }

                return mainCamera;
            }
        }
        private float CameraHeight => MainCamera.orthographicSize;
        private float CameraWidth => CameraHeight * MainCamera.aspect;

        private void Awake()
        {
            initialPosition = cameraTransform.position;
            IsScrollingEnabled = enableCameraDragging;
        }

        private void OnEnable()
        {
            SubscriptionPopup.OnAnyPopupOpened += DisableScrollingWhenPopupIsOpened;
        }

        private void OnDisable()
        {
            SubscriptionPopup.OnAnyPopupOpened -= DisableScrollingWhenPopupIsOpened;
        }
        
        private void Update()
        {
            if (!isDragging)
            {
                if (IsScrollingEnabled && InputProvider.TryGetTouchScreenPos(out currentTouchScreenPosition))
                {
                    IsScrolling = true;
                    isDragging = true;
                    touchPosition = MainCamera.ScreenToWorldPoint(currentTouchScreenPosition);
                }
            }
            else
            {
                isDragging = IsScrollingEnabled && InputProvider.TryGetTouchScreenPos(out currentTouchScreenPosition);
            }
        }

        private void LateUpdate()
        {
            Vector3 _targetPosition = cameraTransform.position;
            if (isDragging)
            {
                Vector3 _difference = MainCamera.ScreenToWorldPoint(currentTouchScreenPosition) - cameraTransform.position;
                Vector3 _newPosition = touchPosition - _difference;
                if (!verticalScrollEnabled)
                {
                    _newPosition.y = cameraTransform.position.y;
                }

                if (!horizontalScrollEnabled)
                {
                    _newPosition.x = cameraTransform.position.x;
                }

                _targetPosition = TryClampPosition(_newPosition);
                if (enableSmoothDamping)
                {
                    MoveCameraSmoothDamp(_targetPosition, DRAGGING_SMOOTH_TIME);
                }
                else
                {
                    MoveCamera(_targetPosition);
                }
            }
            else if(CanDecelerateScroll(_targetPosition))
            {
                MoveCameraSmoothDamp(_targetPosition, RELEASE_SMOOTH_TIME);
            }
            else
            {
                IsScrolling = false;
            }
        }

        public void TryAdjustCameraXBounds(float _xSize)
        {
            if (!cameraConfiner || _xSize <= 0.0f)
            {
                return;
            }

            Vector2 _newSize = cameraConfiner.size;
            _newSize.x = _xSize;
            cameraConfiner.size = _newSize;
        }

        public void ZoomCamera(Vector3 _zoomTargetPosition, Ease _zoomEaseType = Ease.OutSine, Action _onZoomCompleted = null)
        {
            Vector3 _adjustedTargetPosition = _zoomTargetPosition;
            _adjustedTargetPosition.z = initialPosition.z;
            float _zoomStart = CameraHeight;

            moveTween.Stop();
            zoomTween.Stop();
            moveTween = Tween.Position(cameraTransform, _adjustedTargetPosition, zoomDurationSeconds, Ease.OutSine);
            zoomTween = Tween.CameraOrthographicSize(MainCamera, _zoomStart, zoomTargetAmount, zoomDurationSeconds, _zoomEaseType);
            if (_onZoomCompleted != null)
            {
                zoomTween.OnComplete(_onZoomCompleted);
            }
        }

        private Vector3 TryClampPosition(Vector3 _position)
        {
            Vector3 _clampedPosition = _position;
            if (!cameraConfiner)
            {
                return _clampedPosition;
            }

            _clampedPosition.x = Mathf.Clamp(_position.x, cameraConfiner.bounds.min.x + CameraWidth, cameraConfiner.bounds.extents.x - CameraWidth);
            _clampedPosition.y = Mathf.Clamp(_position.y, cameraConfiner.bounds.min.y + CameraHeight, cameraConfiner.bounds.extents.y - CameraHeight);
            return _clampedPosition;
        }

        private void MoveCamera(Vector3 _targetPosition)
        {
            cameraTransform.position = _targetPosition;
        }
        
        private void MoveCameraSmoothDamp(Vector3 _targetPosition, float _smoothTime)
        {
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, _targetPosition, ref velocity, _smoothTime);
        }
        
        private void DisableScrollingWhenPopupIsOpened(SubscriptionPopup _popup)
        {
            currentOpenedPopup = _popup;
            currentOpenedPopup.OnClosed += HandlePopupClosed;
        }

        private void HandlePopupClosed()
        {
            currentOpenedPopup.OnClosed -= HandlePopupClosed;
            currentOpenedPopup = null;
        }
        
        private bool CanDecelerateScroll(Vector3 _targetPosition)
        {
            return enableSmoothDamping && Vector3.Distance(cameraTransform.position, _targetPosition) < DISTANCE_ERROR && velocity.magnitude > VEL_ERROR_SQR;
        }
    }
}