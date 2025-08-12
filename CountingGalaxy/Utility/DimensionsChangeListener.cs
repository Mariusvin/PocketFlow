using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public class DimensionsChangeListener : UIBehaviour
    {
        public static event Action OnDimensionsChanged;
        private static bool isInitialized;

        private bool isDisabled;

        protected override void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            base.Awake();
            if (isInitialized)
            {
                Destroy(this);
            }
            else
            {
                isInitialized = true;
                DontDestroyOnLoad(this);
            }
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            isDisabled = false;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            isDisabled = true;
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            base.OnRectTransformDimensionsChange();

            if (!isDisabled)
            {
                OnDimensionsChanged?.Invoke();
            }
        }
    }
}