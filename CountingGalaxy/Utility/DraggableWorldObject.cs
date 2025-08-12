using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public class DraggableWorldObject : AnimatableButton
    {
        [Header("Draggable Object Settings")] 
        [SerializeField] private float zOffsetWhenDragging = -1f;
        [SerializeField] private bool strictZOffset = true; // if true, zOffsetWhenDragging will be used as the actual point in Z axis when dragging

        [Header("Return Settings")]
        [SerializeField] protected bool returnOnRelease;
        [SerializeField] protected float returnDuration = 0.65f;
        
        public event Action<Transform> OnDragStart;
        public event Action<Transform> OnDragEnd;

        private const float RETURN_DISTANCE_ERR = 0.01f;
        private const float DRAG_CONFIRM_TIME_THRESH = 0.01f;
        
        protected Vector3 initPos;
        private DraggableObjectState currentState;
        private Vector2 offset;
        private float holdTime;
        private float returnTime;

        public bool IsIdle => currentState == DraggableObjectState.Idle;
        
        public bool IsDragging => currentState == DraggableObjectState.Dragging;

        public bool CanDrag { get; set; } = true;

        protected float ZDragOffset => zOffsetWhenDragging;
        
        protected override void Awake()
        {
            base.Awake();
            initPos = transform.position;
            currentState = DraggableObjectState.Idle;
        }

        protected override void Update()
        {
            switch (currentState)
            {
                case DraggableObjectState.Idle:
                    UpdateIdleState();
                    break;
                case DraggableObjectState.Holding:
                    UpdateHoldingState();
                    break;
                case DraggableObjectState.Dragging:
                    UpdateDraggingState();
                    break;
                case DraggableObjectState.Returning:
                    UpdateReturningState();
                    break;
            }
            
            base.Update();
        }
        
        public override void OnPointerDown(PointerEventData _eventData)
        {
            base.OnPointerDown(_eventData);
            if (!CanDrag)
            {
                return;
            }
            
            if (IsInteractive)
            {
                BeginHolding(_eventData.pointerCurrentRaycast.screenPosition);
            }
        }

        public void CacheInitPosition()
        {
            initPos = transform.position;
        }
        
        protected virtual void BeginHolding(Vector2 _screenPos)
        {
            currentState = DraggableObjectState.Holding;
            holdTime = 0f;
            Vector2 _curObjScreenPos = MainCamera.WorldToScreenPoint(transform.position);
            offset = _screenPos - _curObjScreenPos;
        }
        
        protected virtual void BeginIdle()
        {
            currentState = DraggableObjectState.Idle;
        }

        protected virtual void BeginReturning()
        {
            returnTime = 0f;
            currentState = DraggableObjectState.Returning;
        }
        
        protected virtual void BeginDragging()
        {
            currentState = DraggableObjectState.Dragging;
            OnDragStart?.Invoke(transform);
        }
        
        protected virtual void EndDragging()
        {
            if (returnOnRelease)
            {
                BeginReturning();
            }
            else
            {
                BeginIdle();
            }

            OnDragEnd?.Invoke(transform);
        }
        
        protected virtual void UpdateHoldingState()
        {
            if (isPointerDown)
            {
                holdTime += Time.deltaTime;
                if(holdTime >= DRAG_CONFIRM_TIME_THRESH && IsPointerOutOfBounds)
                {
                    BeginDragging();
                }
            }
            else
            {
                BeginIdle();
            }
        }

        protected virtual void UpdateDraggingState()
        {
            if (!InputProvider.TryGetTouchScreenPos(out Vector2 _curScreenTouchPos))
            {
                EndDragging();
                return;
            }
            
            Vector3 _targetPos = MainCamera.ScreenToWorldPoint(_curScreenTouchPos - offset);
            if (strictZOffset)
            {
                _targetPos.z = zOffsetWhenDragging;
            }
            else
            {
                _targetPos.z = initPos.z + zOffsetWhenDragging;
            }
                
            if(CanClick && IsPointerOutOfBounds) // Cancel click if moved
            {
                Debug.Log($"Click cancelled. Reason: CanClick = {CanClick} || IsPointerInBounds = {IsPointerOutOfBounds}");
                CanClick = false;
            }
                
            transform.position = _targetPos;
        }

        protected virtual void UpdateReturningState()
        {
            if ((transform.position - initPos).sqrMagnitude > RETURN_DISTANCE_ERR)
            {
                returnTime += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, initPos, returnTime / returnDuration);
            }
            else
            {
                transform.position = initPos;
                BeginIdle();
            }
        }
        
        protected virtual void UpdateIdleState() { }

        private enum DraggableObjectState
        {
            Idle,
            Holding,
            Dragging,
            Returning
        }
    }
}