using System.Collections;
using System;
using UnityEngine;
using PrimeTween;
using SpineAnimations.Fluvsie;
using Utility;
using Audio;
using Random = UnityEngine.Random;

namespace Activities.Shared.UI
{
    public class EndScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showBackgroundOverlay = true;
        [SerializeField] private bool showWinFluvsie = true;
        [SerializeField] private bool playWinParticles = true;
        [SerializeField] private float endDurationSeconds = 3.5f;

        [Header("References")]
        [SerializeField] private BackgroundOverlay backgroundOverlay;
        [SerializeField] private Transform rayTransform;
        [SerializeField] private FluvsieUISpineController fluvsieSpineController;

        [Header("Particle References")]
        [SerializeField] private Transform particlesParent;
        [SerializeField] private ParticleSystem winParticles;

        private const float SCALE_DURATION_SECONDS = 1.0f;
        private const float RAY_ROTATION_DURATION_SECONDS = 2.4f;
        private const float OVERLAY_FADE_DURATION_SECONDS = 0.75f;
        private readonly Vector3 RAY_TARGET_ROTATION = new Vector3(0.0f, 0.0f, 360.0f);

        private event Action OnComplete;

        private bool isSkipping;
        private Tween rayRotateTween;
        private Tween rayScaleTween;
        private Tween fluvsieScaleTween;
        private Coroutine endScreenCoroutine;

        private void OnEnable()
        {
            backgroundOverlay.AddTriggerAction(Skip);
        }

        private void OnDisable()
        {
            backgroundOverlay.RemoveTriggerAction(Skip);
            OnComplete = null;
        }

        private void Awake()
        {
            fluvsieSpineController.Initialize();
        }

        public void Show(Action _onComplete = null)
        {
            OnComplete = _onComplete;
            BeginShowEndScreen();
        }

        private void Skip()
        {
            if (isSkipping)
            {
                return;
            }

            isSkipping = true;
            StopCoroutine(endScreenCoroutine);
            endScreenCoroutine = null;
            OnComplete?.Invoke();
        }

        private void BeginShowEndScreen()
        {
            if (endScreenCoroutine != null)
            {
                StopCoroutine(endScreenCoroutine);
            }

            endScreenCoroutine = StartCoroutine(ShowEndScreen());
        }

        private IEnumerator ShowEndScreen()
        {
            TryShowOverlay();
            TryAnimateRay();
            TryShowFluvsie();
            TryPlayParticles();
            PlaySounds();

            yield return Yielder.Wait(endDurationSeconds);
            endScreenCoroutine = null;
            OnComplete?.Invoke();
        }

        private void TryShowOverlay()
        {
            if (!showBackgroundOverlay)
            {
                backgroundOverlay.IsRaycastTarget = true; // Enable overlay click to skip even though overlay isn't shown
                return;
            }

            backgroundOverlay.EnableOverlay(OVERLAY_FADE_DURATION_SECONDS);
        }

        private void TryAnimateRay()
        {
            if (!showWinFluvsie)
            {
                return;
            }

            Vector3 _randomTargetRotation = Random.value >= 0.5f ? RAY_TARGET_ROTATION : RAY_TARGET_ROTATION * -1.0f;

            rayRotateTween.Stop();
            rayScaleTween.Stop();
            rayRotateTween = Tween.LocalEulerAngles(rayTransform, Vector3.zero, _randomTargetRotation, RAY_ROTATION_DURATION_SECONDS, Ease.Linear, -1, CycleMode.Incremental);
            rayScaleTween = Tween.Scale(rayTransform, Vector3.one, SCALE_DURATION_SECONDS, Ease.OutBack);
        }

        private void TryShowFluvsie()
        {
            if (!showWinFluvsie)
            {
                return;
            }

            fluvsieSpineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.HappyMix, true);
            fluvsieScaleTween.Stop();
            fluvsieScaleTween = Tween.Scale(fluvsieSpineController.transform, Vector3.one, SCALE_DURATION_SECONDS, Ease.OutBack);
        }

        private void TryPlayParticles()
        {
            if (!playWinParticles)
            {
                return;
            }

            ParticlesPlayer.PlayParticlesSimple(winParticles, particlesParent.position, Color.white, _parent: particlesParent);
        }

        private void PlaySounds()
        {
            AudioController.TryPlayGenericSoundByName(GenericSoundName.LevelComplete);
            AudioController.TryPlayGenericSoundByName(GenericSoundName.ClapJingle);
        }
    }
}