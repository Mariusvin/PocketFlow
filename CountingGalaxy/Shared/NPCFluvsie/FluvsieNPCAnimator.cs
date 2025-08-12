using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using SpineAnimations;
using SpineAnimations.Fluvsie;
using UnityEngine;
using Utility;

namespace Activities.Shared.NPCFluvsie
{
    public class FluvsieNPCAnimator : MonoBehaviour
    {
        [SerializeField] protected FluvsieWorldSpineController spineController;
        [SerializeField] private List<NPCResponseAnimationData> responseAnimations;
        [SerializeField] private bool randomizeSkin;
        [SerializeField] private bool selfInit = true;
        [SerializeField] private bool idleOnInitialize = true;
        
        private Dictionary<NPCResponseType, FluvsieSpineAnimationsMixType> responseAnimationsMap;

        protected bool isIdle;
        private bool isInitialized;
        private float idleTime;
        private Transform cachedTransform;
        private Transform transformToFollow;
        
        public FluvsieSkins SkinName => Enum.TryParse(spineController.FluvsieSkin, out FluvsieSkins _skinName) ? _skinName : FluvsieNPCManager.MAIN_CHARACTER;
        
        public bool RandomizeSkin
        {
            get => randomizeSkin;
            set
            {
                randomizeSkin = value;
                if (randomizeSkin)
                {
                    SetRandomSkin(FluvsieNPCManager.MAIN_CHARACTER);
                }
            }
        }

        public Bone Bone => spineController.Bone;
        
        public Vector3 WorldPosition
        {
            get => CachedTransform.position;
            set => CachedTransform.position = value;
        }
        
        public Vector3 LocalPosition
        {
            get => CachedTransform.localPosition;
            set => CachedTransform.localPosition = value;
        }
        
        public Vector3 LocalScale
        {
            get => CachedTransform.localScale;
            set => CachedTransform.localScale = value;
        }
        
        private Transform CachedTransform
        {
            get
            {
                if (!cachedTransform)
                {
                    cachedTransform = transform;
                }
                return cachedTransform;
            }
        }

        private FluvsieSpineAnimationsMixType CurrentPlayingMix
        {
            set
            {
                switch (value)
                {
                    case FluvsieSpineAnimationsMixType.IdleStandingFrontal:
                    case FluvsieSpineAnimationsMixType.IdleStandingMix:
                    case FluvsieSpineAnimationsMixType.IdleSittingMix:
                        isIdle = true;
                        break;
                    default:
                        isIdle = false;
                        break;
                }
            }
        }

        private void OnEnable()
        {
            spineController.AddAnimationUpdateLocalDelegate(UpdateBoneLocalPosition);
        }

        private void OnDisable()
        {
            spineController.RemoveAnimationUpdateLocalDelegate(UpdateBoneLocalPosition);
        }

        private void Start()
        {
            if (selfInit)
            {
                Initialize();
                if (randomizeSkin)
                {
                    SetRandomSkin();
                }
            }
        }

        private void Update()
        {
            if (isIdle)
            {
                idleTime -= Time.deltaTime;
                if(idleTime <= 0)
                {
                    PlayRandomIdleAnimation();
                }
            }
        }

        /// <summary>
        /// Initializes the NPC animator.
        /// Avoid calling this method in the Awake().
        /// </summary>
        public virtual void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            isIdle = idleOnInitialize;
            spineController.Initialize();
        }

        public void PlayReaction(NPCResponseType _responseType, bool _idleAfterPlaying = true, Action _onReactionFinished = null)
        {
            if (responseAnimationsMap == null)
            {
                InitializeMap();
            }
            
            if (responseAnimationsMap.TryGetValue(_responseType, out FluvsieSpineAnimationsMixType _animationsMixType))
            {
                spineController.SetEmptyAnimation((int)SpineAnimationTrack.Emotion);
                spineController.PlayRandomAnimation(_animationsMixType, false, OnReactionFinished);
                CurrentPlayingMix = _animationsMixType;
            }

            // Local method
            void OnReactionFinished()
            {
                _onReactionFinished?.Invoke();
                if (_idleAfterPlaying)
                {
                    PlayRandomIdleAnimation();
                }
            }
        }

        public void MeetThePlayer(Direction _directionAfterMeeting = Direction.None)
        {
            CurrentPlayingMix = FluvsieSpineAnimationsMixType.Greetings;
            spineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.Greetings, false, OnMeetingFinished);

            // Local method
            void OnMeetingFinished()
            {
                switch (_directionAfterMeeting)
                {
                    case Direction.Left:
                        spineController.PlayAnimation(FluvsieSpineAnimationName.LeftDirectionStanding, true);
                        break;
                    case Direction.Right:
                        spineController.PlayAnimation(FluvsieSpineAnimationName.RightDirectionStanding, true);
                        break;
                }
                PlayRandomIdleAnimation();
            }
        }
        
        public virtual void PlayRandomIdleAnimation()
        {
            spineController.SetEmptyAnimation((int)SpineAnimationTrack.Emotion); // Clear face expression
            CurrentPlayingMix = FluvsieSpineAnimationsMixType.IdleStandingMix;
            idleTime = spineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.IdleStandingMix, true);
        }
        
        public void PlayWalkRightAnimation()
        {
            CurrentPlayingMix = FluvsieSpineAnimationsMixType.WalkRight;
            spineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.WalkRight, true);
        }

        public void PlayWalkLeftAnimation()
        {
            CurrentPlayingMix = FluvsieSpineAnimationsMixType.WalkLeft;
            spineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.WalkLeft, true);
        }

        public void PlayJumpAnimation()
        {
            CurrentPlayingMix = FluvsieSpineAnimationsMixType.JumpMix;
            spineController.PlayRandomAnimation(FluvsieSpineAnimationsMixType.JumpMix, true);
        }

        public void ForceSimpleIdleAnimation()
        {
            isIdle = false;
            spineController.PlayAnimation(FluvsieSpineAnimationName.IdleStandingMix12, true);
        }

        public void StopAllAnimations()
        {
            isIdle = false;
            spineController.ClearAllTracks();
            StopAnimationSound();
        }

        public void StopAnimationSound()
        {
            spineController.TryStopAnimationSound();
        }

        public void SetSkin(FluvsieSkins _skinName)
        {
            spineController.SetSkin(_skinName);
        }
        
        public void BeginFollowingWithBone(Transform _transformToFollow)
        {
            if (spineController.Bone == null)
            {
                Debug.LogError("FluvsieNPCAnimator: Cannot begin following, Bone is null.");
                return;
            }
            
            transformToFollow = _transformToFollow;
            ForceSimpleIdleAnimation();
        }
        
        public void StopFollowingWithBone()
        {
            if (spineController.Bone == null)
            {
                Debug.LogError("FluvsieNPCAnimator: Cannot stop following, Bone is null.");
                return;
            }
            
            transformToFollow = null;
            spineController.Bone.SetLocalPosition(Vector3.zero);
        }

        private void UpdateBoneLocalPosition(ISkeletonAnimation _skeletonAnimation)
        {
            if (!transformToFollow || spineController.Bone == null)
            {
                return;
            }

            Vector3 _targetPosition = spineController.transform.InverseTransformPoint(transformToFollow.position);
            spineController.Bone.SetLocalPosition(_targetPosition);
        }
        
        private void SetRandomSkin(params FluvsieSkins[] _excludedSkins)
        {
            spineController.SetRandomSkin(_excludedSkins);
        }
        
        private void InitializeMap()
        {
            responseAnimationsMap = new Dictionary<NPCResponseType, FluvsieSpineAnimationsMixType>();
            foreach (NPCResponseAnimationData _data in responseAnimations)
            {
                responseAnimationsMap.TryAdd(_data.type, _data.animationsMixType);
            }
        }
        
        [Serializable]
        private struct NPCResponseAnimationData
        {
            public NPCResponseType type;
            public FluvsieSpineAnimationsMixType animationsMixType;
        }
    }
}
