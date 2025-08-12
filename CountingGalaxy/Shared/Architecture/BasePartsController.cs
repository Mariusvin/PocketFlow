using System;
using System.Collections.Generic;
using Activities.Shared.Data;
using UnityEngine;

namespace Activities.Shared.Architecture
{
    // Base class for multiple-parts activities
    // Use it to start and complete activity parts
    // The derived classes should implement the specific behavior for each activity part.
    // Don't forget to Initialize the parts in the derived class=)
    public abstract class BasePartsController<TActivityPartBase, TPartsNameEnum> : MonoBehaviour 
        where TActivityPartBase : ActivityPartBase<TPartsNameEnum> // Base class for activity part
        where TPartsNameEnum : Enum // Enum type for the activity part names (used for tracking, narration, etc.)
    {
        [SerializeField] protected List<TActivityPartBase> activityParts;
        
        private event Action<TPartsNameEnum, int> OnPartStarted; // PartType, PartIndex
        private event Action<TPartsNameEnum, int> OnPartCompleted; // PartType, PartIndex
        private event Action OnAllPartsCompleted;
        
        protected int currentPartIndex;
        protected int currentPartMistakesCount;
        protected BaseDifficultyData difficultyData;
        
        protected bool HasActivityStarted { get; set; }

        public int PartsCount => activityParts.Count;
        
        public int CurrentPartIndex => currentPartIndex;

        public float CurrentPartsProgress => (currentPartIndex + 1) / (float)activityParts.Count;

        public bool IsLastPart => currentPartIndex == activityParts.Count - 1;
        
        protected virtual bool CanStartNextPart => currentPartIndex < activityParts.Count;
        
        protected ActivityPartBase<TPartsNameEnum> PreviousPart => currentPartIndex - 1 >= 0 ? activityParts[currentPartIndex - 1] : null;
        
        protected ActivityPartBase<TPartsNameEnum> NextPart => CanStartNextPart ? activityParts[currentPartIndex] : null;

        protected List<TActivityPartBase> ActivityParts
        {
            get => activityParts;
            set => activityParts = value;
        }
        
        public void Begin()
        {
            TryStartNextPart();
            HasActivityStarted = true;
        }
        
        public virtual void Initialize(Action<TPartsNameEnum, int> _onPartStarted, Action<TPartsNameEnum, int> _onPartCompleted, Action _onAllPartsCompleted, BaseDifficultyData _difficultyData)
        {
            difficultyData = _difficultyData;
            currentPartIndex = 0;
            OnPartStarted = _onPartStarted;
            OnPartCompleted = _onPartCompleted;
            OnAllPartsCompleted = _onAllPartsCompleted;
            
            foreach (TActivityPartBase _activityPart in activityParts)
            {
                _activityPart.Initialize();
            }
        }
        
        protected virtual bool TryStartNextPart()
        {
            if (!CanStartNextPart)
            {
                OnAllPartsCompleted?.Invoke();
                return false;
            }

            activityParts[currentPartIndex].Begin(HandlePartCompleted);
            OnPartStarted?.Invoke(activityParts[currentPartIndex].PartType, currentPartIndex);
            return true;
        }

        protected virtual void HandlePartCompleted(ActivityPartBase<TPartsNameEnum> _activityPart)
        {
            OnPartCompleted?.Invoke(_activityPart.PartType, currentPartIndex);
            currentPartIndex++;
            TryStartNextPart();
        }
    }
} 