using System;
using Utility;

namespace Activities.Shared
{
    /// <summary>
    /// Simple module for handling narration generic voiceover callbacks in the game
    /// </summary>
    public class NarrationRequests : MonoSingleton<NarrationRequests>
    {
        public static event Action OnVoiceOverStopRequested;
        public static event Action<int, Action> OnPartAnnounced;
        public static event Action<bool, Action> OnPositiveFeedbackRequest;
        public static event Action<bool, Action> OnNegativeFeedbackRequest;
        public static event Action<NumberName, bool, float, Action> OnNumberAnnounced; // 1 - name, 2 - isPlural, 3 - delay before next number, 4 - callback after all numbers are announced
        public static event Action<ColorName, bool, float, Action> OnColorAnnounced;
        
        protected override void OnDestroy()
        {
            ClearCallbacks();
            base.OnDestroy();
        }

        public static void AnnouncePart(int _partNumber, Action _onComplete)
        {
            OnPartAnnounced?.Invoke(_partNumber, _onComplete);
        }

        public static void GivePositiveFeedback(bool _force, Action _onComplete)
        {
            OnPositiveFeedbackRequest?.Invoke(_force, _onComplete);
        }

        public static void GiveNegativeFeedback(bool _force, Action _onComplete)
        {
            OnNegativeFeedbackRequest?.Invoke(_force, _onComplete);
        }

        public static void AnnounceNumber(NumberName _voiceOverName, bool _withDelay = false, float _delayDuration = 0f, Action _onComplete = null)
        {
            OnNumberAnnounced?.Invoke(_voiceOverName, _withDelay, _delayDuration, _onComplete);
        }

        public static void AnnounceColor(ColorName _color, bool _withDelay = false, float _delayDuration = 0f, Action _onComplete = null)
        {
            OnColorAnnounced?.Invoke(_color, _withDelay, _delayDuration, _onComplete);
        }

        public static void StopCurrentVO()
        {
            OnVoiceOverStopRequested?.Invoke();
        }

        private static void ClearCallbacks()
        {
            OnVoiceOverStopRequested = null;
            OnPartAnnounced = null;
            OnPositiveFeedbackRequest = null;
            OnNegativeFeedbackRequest = null;
            OnNumberAnnounced = null;
            OnColorAnnounced = null;
        }
    }
} 