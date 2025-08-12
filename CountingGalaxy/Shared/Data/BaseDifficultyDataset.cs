using UnityEngine;

namespace Activities.Shared.Data
{
    [CreateAssetMenu(fileName = "DifficultyDataset", menuName = "Activities/DifficultyDataset")]
    public class BaseDifficultyDataset : ScriptableObject
    {
        [SerializeField] protected BaseDifficultyData[] difficultyData;

        /// <summary>
        /// Returns the difficulty data for the current level.
        /// </summary>
        public BaseDifficultyData GetDifficulty(int _currentLevel)
        {
            if(difficultyData.Length == 0)
            {
                Debug.LogError("No difficulty settings found.");
                return null;
            }
            
            int _diff = int.MaxValue;
            bool _found = false;
            BaseDifficultyData _difficultyData = difficultyData[0];
            foreach (BaseDifficultyData _data in difficultyData)
            {
                if(_currentLevel >= _data.CompletedLevelsThreshold && _currentLevel - _data.CompletedLevelsThreshold < _diff)
                {
                    _diff = _currentLevel - _data.CompletedLevelsThreshold;
                    _difficultyData = _data;
                    _found = true;
                }
            }
            
            if(!_found)
            {
                Debug.LogError("Difficulty settings not found for level: " + _currentLevel + " | returning last difficulty data.");
                return difficultyData[^1];
            }
            else
            {
                return _difficultyData;
            }
        }
    }
}
