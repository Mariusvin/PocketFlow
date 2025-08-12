using System.Text.RegularExpressions;
using Activities.Shared.Architecture;
using Audio.Data;
using UnityEngine;
using Utility.CustomAttributes;

namespace Activities.Shared.Data
{
    // Default activity data class. Used for most activities
    
    [CreateAssetMenu(menuName = "Activities/Base Activity Data")]
    public class BaseActivityData : ScriptableObject
    {
        [field: Header("AVAILABILITY"), SerializeField] public bool IsAvailable { get; private set; } = true; // Determines if activity can be entered from Main Map
        [field: SerializeField] public bool IsTrialActivity { get; private set; } // Determines if activity can be played without subscription
        [field: Range(0, 100), SerializeField] public int TrialPlayCount { get; private set; } // How many times a day activity can be played without subscription. 0 = unlimited

        [field: Header("MAIN"), SerializeField] public ActivityName ActivityName { get; private set; }
        [field: SerializeField] public Scenes Scene { get; private set; }
        [SerializeField] private BaseDifficultyDataset difficultyDataset; // Changed from property for SerializedProperty.FindProperty() method to work
        [field: SerializeField] public bool RepeatAfterComplete { get; private set; } = true; // Determines if activity's scene should be reloaded after completion. If false, the player will be returned to MainMap

        [field: Header("VISUAL"), SerializeField] public Color AccentColor { get; private set; } = Color.magenta;
        
        [PreviewSprite] [SerializeField] private Sprite activityIcon;
        
        [field: SerializeField] public SkillCategoryName SkillsCategory { get; private set; }
        
        [field: Header("AUDIO"), SerializeField] public ActivityAudioData AudioData { get; private set; }

        public BaseDifficultyDataset DifficultyDataset => difficultyDataset;
        
        public Sprite ActivityIcon => activityIcon;
        
        public string GetActivityNameSplitString => Regex.Replace(ActivityName.ToString(), "(?<!^)([A-Z])", " $1"); // Split camel case

        public int SceneIndex => (int)Scene;
    }
}