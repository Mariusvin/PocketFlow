using System;
using UnityEngine;

namespace Utility.IntroSequence
{
    public abstract class IntroSequenceElementBase : MonoBehaviour
    {
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float beginTime; // Time to wait before starting the element
        [SerializeField] private float duration;  // Time to complete the element
        [SerializeField] private bool disableOnComplete = true; // Disable the element after completion

        private event Action<IntroSequenceElementBase> OnComplete;
        private ElementState state;
        
        private float timeFromStart;
        
        /// <summary>
        /// Initializes the element. Should be called before Begin().
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Action that should be performed during the element's duration. Called every frame.
        /// </summary>
        /// <param name="_completedFrac"> Progress completed 0.0 - 1.0 </param>
        protected abstract void StepAction(float _completedFrac);
        
        /// <summary>
        /// Begins the element's sequence.
        /// </summary>
        public virtual void Begin(Action<IntroSequenceElementBase> _onComplete)
        {
            OnComplete = _onComplete;
            state = ElementState.Waiting;
            StepAction(0f);
        }

        private void Update()
        {
            switch (state)
            {
                case ElementState.Stopped:
                    break;
                case ElementState.Waiting:
                    timeFromStart += Time.deltaTime;
                    if (timeFromStart >= beginTime)
                    {
                        timeFromStart = 0;
                        state = ElementState.Running;
                    }
                    break;
                case ElementState.Running:
                    timeFromStart += Time.deltaTime;
                    float _completedFrac = curve.Evaluate(timeFromStart / duration);
                    StepAction(_completedFrac);
                    
                    if (timeFromStart >= duration)
                    {
                        Complete();
                    }
                    break;
            }
        }
        
        protected virtual void Complete()
        {
            state = ElementState.Stopped;
            OnComplete?.Invoke(this);
            enabled = !disableOnComplete;
        }

        private enum ElementState
        {
            Stopped,
            Waiting,
            Running
        }
    }
}
