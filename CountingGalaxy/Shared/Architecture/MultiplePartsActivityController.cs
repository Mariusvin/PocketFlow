using System;
using System.Collections.Generic;
using Activities.Shared.Data;
using Analytics;
using UnityEngine;

namespace Activities.Shared.Architecture
{
    // Base class for multiple-parts activities
    public abstract class MultiplePartsActivityController<TBaseActivityData, TActivityPartBase, TPartNameEnum> : BaseActivityController<TBaseActivityData> 
        where TBaseActivityData : BaseActivityData // Base class for activity data
        where TActivityPartBase : ActivityPartBase<TPartNameEnum> // Base class for activity part
        where TPartNameEnum : Enum // Enum type for the activity part names
    {
        [Header("Multiple Parts Activity Scene References")]
        [SerializeField] protected BasePartsController<TActivityPartBase, TPartNameEnum> partsController;

        private static List<bool> partIntroductionPlayed = new(); // Static tracker - if the part introduction has been played this session

        private const string STARTED_LABEL_SUFFIX = "_Started";
        private const string COMPLETED_LABEL_SUFFIX = "_Completed";
        
        protected int CurrentPartIndex => partsController.CurrentPartIndex;
        
        protected override void Initialize()
        {
            base.Initialize();
            partsController.Initialize(HandlePartStarted, HandlePartCompleted, HandleAllPartsCompleted, currentDifficultyData);
            partIntroductionPlayed ??= new List<bool>(new bool[partsController.PartsCount]);
        }
        
        protected override void EndActivityOnExit()
        {
            base.EndActivityOnExit();
            partIntroductionPlayed = null;
        }
        
        protected virtual void HandlePartStarted(TPartNameEnum _partName, int _partIndex)
        {
            AnalyticsDropOffTracker.TryTrackActivityDropOffPoint(activityData.ActivityName, currentDifficultyData.DifficultyRating, _partName + STARTED_LABEL_SUFFIX, CurrentPlaytimeSeconds);
            if (partIntroductionPlayed != null && partIntroductionPlayed.Count > _partIndex && !partIntroductionPlayed[_partIndex])
            {
                narratorPlayer.PlayPartAnnouncement(_partIndex);
                partIntroductionPlayed[_partIndex] = true;
            }
        }
        
        protected virtual void HandlePartCompleted(TPartNameEnum _partName, int _partIndex)
        {
            AnalyticsDropOffTracker.TryTrackActivityDropOffPoint(activityData.ActivityName, currentDifficultyData.DifficultyRating, _partName + COMPLETED_LABEL_SUFFIX, CurrentPlaytimeSeconds);
            UpdateProgress(partsController.CurrentPartsProgress);
        }
        
        protected virtual void HandleAllPartsCompleted()
        {
            EndActivitySuccess();
        }
        
        protected bool TryConvertPartsController<TPartsController>(out TPartsController _convertedPartsController) where TPartsController : BasePartsController<TActivityPartBase, TPartNameEnum>
        {
            _convertedPartsController = null;
            if (partsController is not TPartsController _controller)
            {
                Debug.LogError("Invalid parts controller type");
                return false;
            }

            _convertedPartsController = _controller;
            return true;
        }
    }
}
