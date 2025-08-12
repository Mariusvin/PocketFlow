using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.UI
{
    [RequireComponent(typeof(Canvas))]
    public class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> targets;
        [SerializeField] private bool updateContinuously = true;
        [SerializeField] private bool bypassActiveInHierarchyCheck;
        [SerializeField] private List<Direction> directionsToApply = new()
        {
            Direction.Left, Direction.Right, Direction.Up, Direction.Down
        };
    
        private Rect lastSafeArea = new(0, 0, 0, 0);
        private Vector2Int lastScreenSize = new(0, 0);
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;
        private Coroutine safeAreaFitterCoroutine;

        private bool HasScreenChanged => 
            lastSafeArea != Screen.safeArea || 
            lastScreenSize != new Vector2Int(Screen.width, Screen.height) || 
            lastOrientation != Screen.orientation;
        
        private void Start()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            if (updateContinuously && HasScreenChanged)
            {
                ApplySafeArea();
            }
        }
        
        public void ApplySafeAreaAfterFrames(int _frames)
        {
            if (safeAreaFitterCoroutine != null)
            {
                StopCoroutine(safeAreaFitterCoroutine);
            }
            
            safeAreaFitterCoroutine = StartCoroutine(ApplySafeAreaAfterFramesCoroutine(_frames));
        }

        private void ApplySafeArea()
        {
            lastSafeArea = Screen.safeArea;
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            lastOrientation = Screen.orientation;

            Vector2 _safeAreaAnchorMin = lastSafeArea.position;
            Vector2 _safeAreaAnchorMax = lastSafeArea.position + lastSafeArea.size;
            if (lastScreenSize is { x: > 0, y: > 0 })
            {
                _safeAreaAnchorMin.x /= lastScreenSize.x;
                _safeAreaAnchorMin.y /= lastScreenSize.y;
                _safeAreaAnchorMax.x /= lastScreenSize.x;
                _safeAreaAnchorMax.y /= lastScreenSize.y;
            }
            else
            {
                Debug.LogError("Cannot apply safe area. Screen size is zero? ");
                return;
            }

            foreach (RectTransform _rt in targets)
            {
                if (!_rt)
                {
                    continue;
                }
                
                if(!bypassActiveInHierarchyCheck && !_rt.gameObject.activeInHierarchy)
                {
                    continue;
                }
                
                Vector2 _newAnchorMin = Vector2.zero;
                Vector2 _newAnchorMax = Vector2.one;

                // Conditionally apply safe area based on the specified directions
                if (directionsToApply.Contains(Direction.Left))
                {
                    _newAnchorMin.x = _safeAreaAnchorMin.x;
                }
                if (directionsToApply.Contains(Direction.Right))
                {
                    _newAnchorMax.x = _safeAreaAnchorMax.x;
                }
                if (directionsToApply.Contains(Direction.Down))
                {
                    _newAnchorMin.y = _safeAreaAnchorMin.y;
                }
                if (directionsToApply.Contains(Direction.Up))
                {
                    _newAnchorMax.y = _safeAreaAnchorMax.y;
                }
                
                _rt.anchorMin = _newAnchorMin;
                _rt.anchorMax = _newAnchorMax;
            }
        }
        
        private IEnumerator ApplySafeAreaAfterFramesCoroutine(int _frames)
        {
            for (int i = 0; i < _frames; i++)
            {
                yield return null;
            }
            
            ApplySafeArea();
            safeAreaFitterCoroutine = null;
        }
    }
}
