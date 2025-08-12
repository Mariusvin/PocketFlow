using UnityEngine;
using Activities.Shared.Data;
using Activities.Shared.UI;
using Audio.Data;
using Utility.UI;
using TutoTOONS;
using Analytics;
using Utility;
using Audio;
using Data;

namespace Activities.Shared.Architecture
{
    // Base class for a single-part activity
    public abstract class BaseActivityController<TBaseActivityData> : MonoBehaviour where TBaseActivityData : BaseActivityData
    {
        [Header("Base Activity Settings")]
        [SerializeField] private bool isMultiTouchEnabled;

        [Header("Base Activity Scene References")]
        [SerializeField] protected BaseActivityUI activityUI;
        [SerializeField] protected EndScreen endScreen;
        [SerializeField] protected NarratorPlayer narratorPlayer;
        
        [Header("Base Activity Project References")]
        [SerializeField] protected TBaseActivityData activityData;
        
        private const string DROP_OFF_POINT_STARTED_LABEL = "Activity_Started";
        private const string DROP_OFF_POINT_COMPLETED_LABEL = "Activity_Completed";
        private const string DROP_OFF_POINT_FAILED_LABEL = "Activity_Exited";
        private const float MIN_LOAD_TIME = 0.1f;
        private const string LOAD_TEXT = " "; // No text when reloading scene

        protected BaseDifficultyData currentDifficultyData;
        protected bool isInitialized;
        protected int mistakesCount;
        private float startTime;

        // TODO: Future update. Will be used for adaptive difficulty system
        protected int CurrentLevel
        {
            get => SavedData.GetInt(activityData.ActivityName + "_Level_" + ProfileDataController.CurrentSelectedProfile.ID, 1);
            set => SavedData.SetInt(activityData.ActivityName + "_Level_" + ProfileDataController.CurrentSelectedProfile.ID, Mathf.Max(value, 1));
        }
        
        protected int CurrentPlaytimeSeconds => Mathf.Abs((int)(Time.time - startTime));

        private ActivityAudioData AudioData => activityData.AudioData;
        
        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            // Implement in derived classes
        }

        protected virtual void OnDisable()
        {
            // Implement in derived classes
        }

        protected virtual void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            else if (!TryCacheActivityDataFromPersistentData())
            {
                Debug.LogError("Failed to convert activity data from persistent data. Activity can't be initialized");
                LoadMainMap();
                return;
            }
            else if (!activityData)
            {
                Debug.LogError("No persistent activity data found and activity data field isn't assigned. Assign activity data in the inspector");
                LoadMainMap();
                return;
            }
            

            startTime = Time.time;
            Input.multiTouchEnabled = isMultiTouchEnabled;
            currentDifficultyData = activityData.DifficultyDataset.GetDifficulty(CurrentLevel);
            activityUI.Initialize(CurrentLevel, currentDifficultyData.DifficultyRating, EndActivityOnExit, HandleRestartButtonClicked, LeaveActivity, OnPreviousButtonClick);
            narratorPlayer.Initialize(AudioData.NarratorData, AudioData.VoiceOversDatasets);
            AudioController.TryInitialize();
            TrackInitialAnalyticsEvents();

            // Loading screen gives some time for plugins to initialize (e.g. spine)
            if (!LoadingScreen.TryHideLoadingScreen(AudioData.TransitionOutSound, ContinueInitializationNextFrame)) 
            {
                DelayedActionsManager.DelayedCall(ContinueInitializationNextFrame, 1);
            }
        }

        protected virtual void ContinueInitializationNextFrame()
        {
            isInitialized = true;
            AudioController.TryPlayBackgroundMusic(AudioData.BackgroundMusic);
        }

        protected bool TryConvertDifficultyData<TDifficultyData>(out TDifficultyData _convertedData) where TDifficultyData : BaseDifficultyData
        {
            _convertedData = null;
            if (currentDifficultyData is not TDifficultyData _data)
            {
                Debug.LogError("Invalid difficulty data type");
                return false;
            }

            _convertedData = _data;
            return true;
        }
        
        protected bool TryConvertActivityData<TActivityData>(out TActivityData _convertedData) where TActivityData : BaseActivityData
        {
            _convertedData = null;
            if (activityData is not TActivityData _data)
            {
                Debug.LogError("Invalid activity data type");
                return false;
            }

            _convertedData = _data;
            return true;
        }
        
        protected void UpdateProgress(float _target)
        {
            activityUI.UpdateProgressBar(_target);
        }

        protected virtual void EndActivitySuccess()
        {
            TrackActivityCompletedAnalyticsEvents();
            
            if (!endScreen)
            {
                LeaveActivity();
                return;
            }
            endScreen.Show(LeaveActivity);
        }

        protected virtual void EndActivityOnExit()
        {
            narratorPlayer.PlayIntroductionOnce = false;
            TrackActivityExitAnalyticsEvents();
            LoadMainMap();
        }
        
        protected virtual void LeaveActivity()
        {
            CurrentLevel++;
            if (!activityData.RepeatAfterComplete)
            {
                LoadMainMap();
            }
            else
            {
                ReloadCurrentScene();
            }
        }
        
        protected void LoadMainMap()
        {
            TrackForceEndAnalyticsEvents();
            if (!LoadingScreen.TryShowLoadingScreen(ContinueLoading))
            {
                ContinueLoading(); // Can't show loading screen?
            }

            // Local method
            void ContinueLoading()
            {
                SceneLoader.LoadSceneByIndex((int)Scenes.MainMap); // Load the main map
            }
        }

        private bool TryCacheActivityDataFromPersistentData()
        {
            if (!PersistentActivityData.IsActivityDataSet)
            {
                return true; // No data set, nothing to cache - check serialized field reference
            }

            if (!PersistentActivityData.TryConvertCurrentActivityData(out TBaseActivityData _activityData))
            {
                return false; // Failed to convert - activity can't be initialized anymore
            }

            activityData = _activityData;
            return true;
        }

        private void HandleRestartButtonClicked()
        {
            TrackActivityExitAnalyticsEvents();
            ReloadCurrentScene();
        }
        
        // In editor only
        private void OnPreviousButtonClick()
        {
            CurrentLevel--;
            ReloadCurrentScene();
        }

        private void ReloadCurrentScene()
        {
            TrackForceEndAnalyticsEvents();
            if (!LoadingScreen.TryShowLoadingScreen(activityData.ActivityIcon, activityData.AccentColor, AudioData.TransitionInSound, SceneLoader.ReloadCurrentScene, MIN_LOAD_TIME, false, LOAD_TEXT)) // Reload the same scene
            {
                SceneLoader.ReloadCurrentScene(); // Can't show loading screen?
            }
        }

        private void TrackInitialAnalyticsEvents()
        {
            AnalyticsTracker.TrackFirstTimeEnterActivity(activityData.ActivityName);
            AnalyticsTracker.TrackActivityStarted(activityData.ActivityName, currentDifficultyData.DifficultyRating);
            AnalyticsDropOffTracker.TryTrackActivityDropOffPoint(activityData.ActivityName, currentDifficultyData.DifficultyRating, DROP_OFF_POINT_STARTED_LABEL, 0);
        }

        private void TrackActivityCompletedAnalyticsEvents()
        {
            AnalyticsTracker.TrackActivityCompleted(activityData.ActivityName, currentDifficultyData.DifficultyRating, CurrentPlaytimeSeconds);
            AnalyticsTracker.TrackActivityMistakes(activityData.ActivityName, currentDifficultyData.DifficultyRating, mistakesCount);
            AnalyticsDropOffTracker.TryTrackActivityDropOffPoint(activityData.ActivityName, currentDifficultyData.DifficultyRating, DROP_OFF_POINT_COMPLETED_LABEL, CurrentPlaytimeSeconds);
        }

        private void TrackActivityExitAnalyticsEvents()
        {
            AnalyticsTracker.TrackActivityExit(activityData.ActivityName, currentDifficultyData.DifficultyRating, CurrentPlaytimeSeconds);
            AnalyticsTracker.TrackActivityMistakes(activityData.ActivityName, currentDifficultyData.DifficultyRating, mistakesCount);
            AnalyticsDropOffTracker.TryTrackActivityDropOffPoint(activityData.ActivityName, currentDifficultyData.DifficultyRating, DROP_OFF_POINT_FAILED_LABEL, CurrentPlaytimeSeconds);
        }

        private void TrackForceEndAnalyticsEvents()
        {
            if (!activityData)
            {
                return;
            }

            AnalyticsTrackerPersistentData.AddTimeSpentInScene(activityData.Scene, CurrentPlaytimeSeconds);
            AnalyticsDropOffTracker.TryStopTrackActivityDropOff(activityData.ActivityName, currentDifficultyData.DifficultyRating);
        }
    }
}