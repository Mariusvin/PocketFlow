using System;
using UnityEngine;
using PrimeTween;

namespace Activities.Shared.Hints
{
    public class HintObject : MonoBehaviour
    {
        private const float SCALE_DURATION_SECONDS = 0.42f;
        private const float PULSATE_DURATION_SECONDS = 0.55f;
        private const float SCALE_MULTIPLIER = 1.2f;

        private Tween scaleTween;

        public bool IsActive
        {
            get => gameObject.activeSelf;

            set
            {
                gameObject.SetActive(value);
                scaleTween.Stop();
            }
        }

        public Vector3 Position
        {
            get => transform.position;

            set
            {
                Vector3 _position = value;
                _position.z = 0.0f; // Keep default Z axis
                transform.position = _position;
            }
        }

        public void Show()
        {
            transform.localScale = Vector3.zero;
            ScaleHint(Vector3.one, SCALE_DURATION_SECONDS, Ease.OutQuart, _onComplete: Pulsate);
        }

        private void Pulsate()
        {
            ScaleHint(Vector3.one * SCALE_MULTIPLIER, PULSATE_DURATION_SECONDS, Ease.InOutSine, -1, CycleMode.Yoyo);
        }

        private void ScaleHint(Vector3 _targetScale, float _durationSeconds, Ease _ease, int _cycles = 1, CycleMode _cycleMode = CycleMode.Restart, Action _onComplete = null)
        {
            scaleTween.Stop();
            scaleTween = Tween.Scale(transform, _targetScale, _durationSeconds, _ease, _cycles, _cycleMode);
            if (_onComplete != null)
            {
                scaleTween.OnComplete(_onComplete);
            }
        }
    }
}