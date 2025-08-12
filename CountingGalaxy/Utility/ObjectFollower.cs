using UnityEngine;

namespace Utility
{
    [ExecuteInEditMode]
    public class ObjectFollower : MonoBehaviour
    {
        // If using for UI elements, highly recommended to use the same canvas space for both objects
        [SerializeField] private Transform target;
        [SerializeField] private bool isUpdating;
        [SerializeField] private bool followInEditMode;
        [SerializeField] private float followSpeed = 1f;

        private const float DIST_ERROR = 0.002f;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        public bool IsUpdating
        {
            get => isUpdating;
            set => isUpdating = value;
        }

        public float FollowSpeed
        {
            get => followSpeed;
            set => followSpeed = value;
        }

        private Vector3 CurrentPosition
        {
            get => transform.position;
            set => transform.position = value;
        }

        private bool IsInstant => FollowSpeed == 0f;
        private Vector3 TargetPosition => Target.position;
        
        private void LateUpdate()
        {
            if (!isUpdating)
            {
                return;
            }
            
            if (!Application.isPlaying && !followInEditMode)
            {
                return;
            }

            if (IsInstant)
            {
                CurrentPosition = target.position;
            }
            else
            {
                Vector3 _curPos = CurrentPosition;
                Vector3 _targetPos = TargetPosition;
                if ((_curPos - _targetPos).sqrMagnitude <= DIST_ERROR)
                {
                    return;
                }
            
                _curPos = Vector3.MoveTowards(_curPos, _targetPos, followSpeed * Time.deltaTime);
                CurrentPosition = _curPos;
            }
        }
    }
}