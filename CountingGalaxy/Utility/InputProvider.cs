#if !UNITY_EDITOR
using System.Collections;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public static class InputProvider
    {
        private static Camera mainCamera;
        private static readonly RaycastHit[] RAYCAST_HITS;
        private static readonly PointerEventData CACHED_POINTER_EVENT_DATA; // stores only pointer position
        private static readonly List<RaycastResult> RAYCAST_RESULTS;
        
        private static readonly int MAX_RAY_INTERSECTIONS = 10;
        
        private static Camera MainCamera => mainCamera ? mainCamera : mainCamera = Camera.main;

        static InputProvider()
        {
            RAYCAST_HITS = new RaycastHit[MAX_RAY_INTERSECTIONS];
            CACHED_POINTER_EVENT_DATA = new PointerEventData(EventSystem.current);
            RAYCAST_RESULTS = new List<RaycastResult>();
        }
        
        public static bool IsTouching()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            return Input.GetMouseButton(0);
#else
            if (Input.touchCount <= 0)
            {
                return false;
            }
            
            Touch _touch = Input.GetTouch(Input.touchCount - 1);
            return _touch.phase != TouchPhase.Ended && _touch.phase != TouchPhase.Canceled;
#endif
        }

        public static bool TryGetTouch(out Vector3 _screenPos)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            _screenPos = Input.mousePosition;
            return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0);
#else
		    if (Input.touchCount > 0)
		    {
			    _screenPos = Input.GetTouch(0).position;
			    return true;
		    }
		    else
		    {
			    _screenPos = Vector3.zero;
			    return false;
		    }
#endif
        }

        public static bool IsTouchingUI<T>(out T _object) where T : Component
        {
            _object = null;
            Vector2 _pointerPos;
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                _pointerPos = Input.mousePosition;
            }
            else
            {
                return false;
            }
#else
            if (Input.touchCount > 0)
            {
                _pointerPos = Input.GetTouch(Input.touchCount - 1).position;
            }
            else
            {
                return false;
            }
#endif

            CACHED_POINTER_EVENT_DATA.position = _pointerPos;
            EventSystem.current.RaycastAll(CACHED_POINTER_EVENT_DATA, RAYCAST_RESULTS);
            foreach (RaycastResult _result in RAYCAST_RESULTS)
            {
                if (_result.gameObject.TryGetComponent(out _object))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool IsTouching<T>(out T _object) where T : Component
        {
            _object = null;
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                //Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction.normalized * 100f, Color.blue, 2f);
                if (Physics.Raycast(_ray, out RaycastHit _hit))
                {
                    _object = _hit.transform.GetComponent<T>();
                    if (_object)
                    {
                        return true;
                    }
                }
            }

            return false;

#else
            if (Input.touchCount > 0)
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
                if (Physics.Raycast(_ray, out RaycastHit _hit))
                {
                    _object = _hit.transform.GetComponent<T>();
                    if (_object)
                    {
                        return true;
                    }
                }
            }
            return false;
#endif
        }
        
        public static bool IsTouching<T>(out List<T> _object) where T : MonoBehaviour
        {
            _object = new List<T>();
            Ray _ray = new();
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            }
#else
            if (Input.touchCount > 0)
            {
                _ray = MainCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
            }
#endif
            
            int _count = Physics.RaycastNonAlloc(_ray, RAYCAST_HITS);
            for (int i = 0; i < _count; i++)
            {
                T _obj = RAYCAST_HITS[i].collider.GetComponent<T>();
                if (_obj)
                {
                    _object.Add(_obj);
                }
            }

            return _object.Count > 0;
        }
        
        public static bool IsTouching<T>(out List<T> _object, out List<Vector3> _touchPoints) where T : MonoBehaviour
        {
            _object = new List<T>();
            _touchPoints = new List<Vector3>();
            Ray _ray = new();
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            }
#else
            if (Input.touchCount > 0)
            {
                _ray = MainCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
            }
#endif
            
            int _count = Physics.RaycastNonAlloc(_ray, RAYCAST_HITS);
            for (int i = 0; i < _count; i++)
            {
                T _obj = RAYCAST_HITS[i].collider.GetComponent<T>();
                if (_obj)
                {
                    _object.Add(_obj);
                    _touchPoints.Add(RAYCAST_HITS[i].point);
                }
            }

            return _count > 0;
        }
        
        public static bool IsObjectTouchedThisFrame<T>(T _target)
        {
            Ray _ray = new();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            }
#else
            if (Input.touchCount > 0)
            {
                _ray = MainCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
            }
#endif
            
            int _count = Physics.RaycastNonAlloc(_ray, RAYCAST_HITS);
            for (int i = 0; i < _count; i++)
            {
                T _obj = RAYCAST_HITS[i].collider.GetComponent<T>();
                if (_obj != null)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool IsScreenTouchedThisFrame()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            return Input.GetMouseButtonDown(0);
#else
            if (Input.touchCount > 0)
            {
                Touch _touch = Input.GetTouch(Input.touchCount - 1);
                return _touch.phase == TouchPhase.Began;
            }
            else
            {
                return false;
            }
#endif
        }
        
        public static bool IsWorldTouchedThisFrame(out Vector3 _worldPos)
        {
            _worldPos = Vector3.zero;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0))
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out RaycastHit _hit, Mathf.Infinity))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }
            return false;
#else
            if (Input.touchCount <= 0)
            {
                return false;
            }
            
            Touch _touch = Input.GetTouch(Input.touchCount - 1);
            if (Input.touchCount > 0 && _touch.phase == TouchPhase.Began)
            {
                Ray _ray = MainCamera.ScreenPointToRay(_touch.position);
                if (Physics.Raycast(_ray, out RaycastHit _hit))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }

            return false;
#endif
        }
        
        public static bool IsWorldTouchedThisFrame(LayerMask _mask, out Vector3 _worldPos)
        {
            _worldPos = Vector3.zero;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0))
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out RaycastHit _hit, Mathf.Infinity, _mask))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }
            return false;
#else
            if (Input.touchCount <= 0)
            {
                return false;
            }
            
            Touch _touch = Input.GetTouch(Input.touchCount - 1);
            if (Input.touchCount > 0 && _touch.phase == TouchPhase.Began)
            {
                Ray _ray = MainCamera.ScreenPointToRay(_touch.position);
                if (Physics.Raycast(_ray, out RaycastHit _hit, Mathf.Infinity, _mask))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }

            return false;
#endif
        }
        
        public static bool TryGetTouchWorldPos(out Vector3 _worldPos)
        {
            _worldPos = Vector3.zero;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out RaycastHit _hit, Mathf.Infinity))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }
            return false;
#else
            if (Input.touchCount > 0)
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
                if (Physics.Raycast(_ray, out RaycastHit _hit))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }
            return false;
#endif
        }
        
        public static bool TryGetTouchWorldPos(LayerMask _layerMask, out Vector3 _worldPos)
        {
            _worldPos = Vector3.zero;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out RaycastHit _hit, Mathf.Infinity, _layerMask))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }
            return false;
#else
            if (Input.touchCount > 0)
            {
                Ray _ray = MainCamera.ScreenPointToRay(Input.GetTouch(Input.touchCount - 1).position);
                if (Physics.Raycast(_ray, out RaycastHit _hit,  Mathf.Infinity, _layerMask))
                {
                    _worldPos = _hit.point;
                    return true;
                }
            }
            return false;
#endif
        }
        
        public static bool TryGetTouchScreenPos(out Vector3 _screenPos)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            _screenPos = Input.mousePosition;
            return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
#else
		    if (Input.touchCount > 0)
		    {
			    _screenPos = Input.GetTouch(Input.touchCount - 1).position;
			    return true;
		    }
		    else
		    {
			    _screenPos = Vector3.zero;
			    return false;
		    }
#endif
        }
        
        public static bool TryGetTouchScreenPos(out Vector2 _screenPos)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            _screenPos = Input.mousePosition;
            return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
#else
		    if (Input.touchCount > 0)
		    {
			    _screenPos = Input.GetTouch(Input.touchCount - 1).position;
			    return true;
		    }
		    else
		    {
			    _screenPos = Vector3.zero;
			    return false;
		    }
#endif
        }
    }
}