using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

namespace Utility.Sprites
{
    public class SpritesBounceAnimator : MonoBehaviour
    {
        [Header("Sprite References")]
        [SerializeField] private List<SpriteRenderer> slotsBottom;
        [SerializeField] private List<SpriteRenderer> slotsCenter;
        [SerializeField] private List<SpriteRenderer> slotsTop;

        private const float JUMP_HEIGHT_MIN = 0.5f;
        private const float JUMP_HEIGHT_MAX = 1.0f;
        private const float ROTATION_ANGLE_MIN = -30.0f;
        private const float ROTATION_ANGLE_MAX = 30.0f;
        private const float BOUNCE_DURATION_MIN = 0.4f;
        private const float BOUNCE_DURATION_MAX = 0.5f;
        private const float DURATION_MULTIPLIER = 0.5f;

        private List<SpriteRenderer> sharedSlots;
        private List<Vector3> sharedSlotsInitialPositions;
        private bool isInitialized;
        private Tween moveTween;
        private Tween rotationTween;
        private Tween scaleTween;

        [ContextMenu("Animate Bouncing")]
        public void AnimateBouncing()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            for (int i = 0; i < sharedSlots.Count; i++)
            {
                float _jumpHeight = Random.Range(JUMP_HEIGHT_MIN, JUMP_HEIGHT_MAX);
                float _rotation = Random.Range(ROTATION_ANGLE_MIN, ROTATION_ANGLE_MAX);
                float _duration = Random.Range(BOUNCE_DURATION_MIN, BOUNCE_DURATION_MAX);

                Transform _slotTransform = sharedSlots[i].transform;
                _slotTransform.localPosition = sharedSlotsInitialPositions[i];
                float _targetYPosition = _slotTransform.localPosition.y + _jumpHeight;
                Vector3 _targetEuler = transform.eulerAngles;
                _targetEuler.z += _rotation;

                moveTween.Stop();
                rotationTween.Stop();
                scaleTween.Stop();
                
                moveTween = Tween.LocalPositionY(_slotTransform, _targetYPosition, _duration * DURATION_MULTIPLIER, Ease.OutSine, 2, CycleMode.Yoyo);
                rotationTween = Tween.Rotation(_slotTransform, _targetEuler, _duration, Ease.OutSine);
                scaleTween = Tween.Scale(_slotTransform, Vector3.one, _duration, Ease.OutExpo);
            }
        }

        private void Initialize()
        {
            sharedSlotsInitialPositions = new List<Vector3>();

            // Randomize shared slots
            sharedSlots = new List<SpriteRenderer>();
            sharedSlots.AddRange(slotsBottom.OrderBy(_slot => Random.value));
            sharedSlots.AddRange(slotsCenter.OrderBy(_slot => Random.value));
            sharedSlots.AddRange(slotsTop.OrderBy(_slot => Random.value));

            // Prepare shared slots
            foreach (SpriteRenderer _slot in sharedSlots)
            {
                _slot.transform.localScale = Vector3.zero;
                sharedSlotsInitialPositions.Add(_slot.transform.localPosition);
            }

            isInitialized = true;
        }
    }
}