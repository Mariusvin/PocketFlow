using System;
using UnityEngine;

namespace Utility.IntroSequence
{
    public class IntroSequence : MonoBehaviour
    {
        [SerializeField] private IntroSequenceElementBase[] sequenceElements;
        
        private event Action OnIntroSequenceEnd;

        private bool isInProgress;
        private int completedCount;

        public bool IsInProgress => isInProgress;

        private void OnDestroy()
        {
            OnIntroSequenceEnd = null;
        }
        
        public void Initialize()
        {
            foreach (IntroSequenceElementBase _element in sequenceElements)
            {
                _element.Initialize();
            }
        }
        
        public void BeginSequence(Action _onComplete = null)
        {
            if(sequenceElements.Length == 0)
            {
                _onComplete?.Invoke();
                return;
            }

            isInProgress = true;
            OnIntroSequenceEnd = _onComplete;
            foreach (IntroSequenceElementBase _element in sequenceElements)
            {
                _element.Begin(RegisterCompleted);
            }
        }

        private void RegisterCompleted(IntroSequenceElementBase _element)
        {
            _element.enabled = false;
            completedCount++;
            if (completedCount == sequenceElements.Length)
            {
                isInProgress = false;
                OnIntroSequenceEnd?.Invoke();
                OnIntroSequenceEnd = null;
            }
        }
    }
}
