using System;
using UnityEngine;

namespace Utility.CameraRelated
{
    [ExecuteAlways]
    public class AlignWithCameraBounds : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private AlignType alignType;
        [SerializeField] private bool alignOnAwake = true;
        [SerializeField] private bool alignOnUpdate = true;
        
        private Transform mainCameraTransform;
        private Transform cachedTransform;

        private Transform MainCamTransform => mainCameraTransform ? mainCameraTransform : Camera.main.transform; // Editor-only
        private Transform CachedTransform => cachedTransform ? cachedTransform : transform;
        
        private void Awake()
        {
            if (!mainCamera)
            {
                mainCamera = Camera.main;
            }

            if (!cachedTransform)
            {
                cachedTransform = transform;
            }

            if (!mainCameraTransform)
            {
                mainCameraTransform = mainCamera.transform;
            }

            if (alignOnAwake)
            {
                Align();
            }
        }

        private void Update()
        {
            if (alignOnUpdate)
            {
                Align();
            }
        }
        
        private void Align()
        {
            switch (alignType)
            {
                case AlignType.Bottom:
                    AlignToTheBottom();
                    break;
                case AlignType.Top:
                    AlignToTheTop();
                    break;
                case AlignType.Left:
                    AlignToTheLeft();
                    break;
                case AlignType.Right:
                    AlignToTheRight();
                    break;
            }
        }

        public void AlignToTheBottom()
        {
            Vector3 _camBottom = MainCamTransform.position - MainCamTransform.up * mainCamera.orthographicSize;
            CachedTransform.position = new Vector3(transform.position.x, _camBottom.y, CachedTransform.position.z);
        }
        
        public void AlignToTheTop()
        {
            Vector3 _camTop = MainCamTransform.position + MainCamTransform.up * mainCamera.orthographicSize;
            CachedTransform.position = new Vector3(transform.position.x, _camTop.y, CachedTransform.position.z);
        }
        
        public void AlignToTheLeft()
        {
            Vector3 _camLeft = MainCamTransform.position - MainCamTransform.right * (mainCamera.orthographicSize * mainCamera.aspect);
            CachedTransform.position = new Vector3(_camLeft.x, transform.position.y, CachedTransform.position.z);
        }
        
        public void AlignToTheRight()
        {
            Vector3 _camRight = MainCamTransform.position + MainCamTransform.right * (mainCamera.orthographicSize * mainCamera.aspect);
            CachedTransform.position = new Vector3(_camRight.x, transform.position.y, CachedTransform.position.z);
        }
        
        private enum AlignType
        {
            Bottom,
            Top,
            Left,
            Right
        }
    }
}