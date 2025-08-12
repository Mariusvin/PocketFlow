using UnityEngine;

namespace Activities.Shared.Data
{
    [CreateAssetMenu(fileName = "BaseDifficultyData", menuName = "Activities/Base Difficulty Data")]
    public class BaseDifficultyData : ScriptableObject
    {
        [SerializeField] protected DifficultyRating difficultyRating; // Using to identify the difficulty. No real purpose at the moment.
        [SerializeField] protected int completedLevelsThreshold;      // The level at which this difficulty is unlocked.
        
        public DifficultyRating DifficultyRating => difficultyRating;
        public int CompletedLevelsThreshold => completedLevelsThreshold;
    }
}
