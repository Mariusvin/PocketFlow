using System;
using Activities.Shared.NPCFluvsie;
using Spine.Unity;
using UnityEngine;

namespace Activities.Shared.Hygiene
{
    public class HygieneFluvsieAnimator : FluvsieNPCAnimator
    {
        [Header("Hygiene Fluvsie References")]
        [SerializeField] private Transform bodyFrontTransform;
        [SerializeField] private AnimationReferenceAsset goodbyeAnimation;

        private const float BODY_FRONT_HIDDEN_Z = -0.01f;

        private float initialBodyFrontZ;

        public override void Initialize()
        {
            base.Initialize();
            initialBodyFrontZ = bodyFrontTransform.localPosition.z;
            SetBodyFrontZ(BODY_FRONT_HIDDEN_Z);
        }

        public void PlayGoodbyeAnimation(Action _onComplete = null)
        {
            isIdle = false;
            spineController.TryPlayAnimation(0, goodbyeAnimation, false, _onComplete);
        }

        public void SetBodyToFront()
        {
            SetBodyFrontZ(initialBodyFrontZ);
        }

        private void SetBodyFrontZ(float _targetZ)
        {
            Vector3 _bodyFrontPosition = bodyFrontTransform.localPosition;
            _bodyFrontPosition.z = _targetZ;
            bodyFrontTransform.localPosition = _bodyFrontPosition;
        }
    }
}