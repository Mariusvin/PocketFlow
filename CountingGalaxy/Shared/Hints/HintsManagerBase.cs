using System.Collections.Generic;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Activities.Shared.Architecture;
using TutoTOONS;
using Utility;

namespace Activities.Shared.Hints
{
    public abstract class HintsManagerBase<T> : MonoSingleton<T> where T : HintsManagerBase<T>
    {
        private const string HINTS_SHOWN_SUFFIX = "_FirstTimeHintsShown";

        private event Action OnPrefabLoadSuccess;

        private readonly Dictionary<ActivityName, bool> hintsShownMap = new Dictionary<ActivityName, bool>();
        private readonly List<HintObject> availableHints = new List<HintObject>();
        private readonly List<HintObject> activeHints = new List<HintObject>();
        private AsyncOperationHandle<GameObject> prefabLoadHandle;
        private GameObject hintObjectPrefab;

        private bool IsPrefabLoaded => hintObjectPrefab != null;

        protected abstract string HintObjectPrefabPath { get; }

        protected abstract void ApplyHintPosition<TPosition>(HintObject _hint, TPosition _position);

        protected override void ValidAwake()
        {
            base.ValidAwake();
            LoadHintObjectPrefab();
        }

        protected override void OnDestroy()
        {
            Addressables.Release(prefabLoadHandle);
            base.OnDestroy();
        }

        public static void SaveHintsShown(ActivityName _activityName, bool _value)
        {
            SavedData.SetInt(_activityName + HINTS_SHOWN_SUFFIX, _value ? 1 : 0);
        }

        public static void ShowHint<TPosition>(TPosition _position, ActivityName _activityName = ActivityName.None)
        {
            if (Instance.AreFirstTimeHintsShown(_activityName))
            {
                return;
            }

            Instance.RequestHint(_position);
        }

        public static void HideAllHints(ActivityName _activityName = ActivityName.None)
        {
            if (Instance.AreFirstTimeHintsShown(_activityName))
            {
                return;
            }

            Instance.HideActiveHints();
        }

        private void LoadHintObjectPrefab()
        {
            prefabLoadHandle = Addressables.LoadAssetAsync<GameObject>(HintObjectPrefabPath);
            prefabLoadHandle.Completed += OnLoadHandleCompleted;
        }

        private void OnLoadHandleCompleted(AsyncOperationHandle<GameObject> _handle)
        {
            prefabLoadHandle.Completed -= OnLoadHandleCompleted;

            if (_handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Hint Object Prefab failed to load");
                return;
            }

            hintObjectPrefab = _handle.Result;
            OnPrefabLoadSuccess?.Invoke();
            OnPrefabLoadSuccess = null;
        }

        private bool AreFirstTimeHintsShown(ActivityName _activityName)
        {
            if (hintsShownMap.Count <= 0)
            {
                MapActivitiesHintsShownData();
            }

            return hintsShownMap.TryGetValue(_activityName, out bool _hintsShown) ? _hintsShown : true;
        }

        private void MapActivitiesHintsShownData()
        {
            Array _activityNames = Enum.GetValues(typeof(ActivityName));
            for (int i = 1; i < _activityNames.Length; i++) // Ignore first enum value 'None'
            {
                ActivityName _activityName = (ActivityName)_activityNames.GetValue(i);
                bool _hintsShown = SavedData.GetInt(_activityName + HINTS_SHOWN_SUFFIX) == 1;
                hintsShownMap.TryAdd(_activityName, _hintsShown);
            }
        }

        private void RequestHint<TPosition>(TPosition _hintPosition)
        {
            if (IsPrefabLoaded)
            {
                TryShowHint(_hintPosition);
            }
            else
            {
                OnPrefabLoadSuccess = OnLoadSuccess;
            }

            // Local method
            void OnLoadSuccess()
            {
                TryShowHint(_hintPosition);
            }
        }

        private void TryShowHint<TPosition>(TPosition _position)
        {
            HintObject _hintObject = GetHintFromPool();
            if (!_hintObject)
            {
                return;
            }

            ApplyHintPosition(_hintObject, _position);
            _hintObject.Show();
        }

        private void HideActiveHints()
        {
            for (int i = activeHints.Count - 1; i >= 0; i--)
            {
                HintObject _activeHint = activeHints[i];
                ReturnHintToPool(_activeHint);
            }
        }

        private HintObject GetHintFromPool()
        {
            HintObject _hintObject;
            if (availableHints.Count > 0)
            {
                int _lastIndex = availableHints.Count - 1;
                _hintObject = availableHints[_lastIndex];
                _hintObject.IsActive = true;
                availableHints.RemoveAt(_lastIndex);
            }
            else
            {
                _hintObject = CreateHintObject();
                if (!_hintObject)
                {
                    return null;
                }
            }
            activeHints.Add(_hintObject);

            return _hintObject;
        }

        private void ReturnHintToPool(HintObject _hintObject)
        {
            if (!_hintObject)
            {
                return;
            }

            _hintObject.IsActive = false;
            activeHints.Remove(_hintObject);
            availableHints.Add(_hintObject);
        }

        private HintObject CreateHintObject()
        {
            GameObject _gameObject = Instantiate(hintObjectPrefab, transform);
            if (!_gameObject.TryGetComponent(out HintObject _hintObject))
            {
                Debug.LogError("Invalid prefab was loaded");
                Destroy(_gameObject);
                return null;
            }
            return _hintObject;
        }
    }
}