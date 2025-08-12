using System.Collections.Generic;
using UnityEngine;

namespace Activities.Shared.JumperAnimal
{
    [CreateAssetMenu(menuName = "Activities/Shared/JumperAnimal/Jumper Animals Dataset")]
    public class JumperAnimalsDataset : ScriptableObject
    {
        [SerializeField] private List<AnimalData> animals;

        public List<AnimalData> Animals => animals;

        public AnimalData GetAnimalData(AnimalName _animalName)
        {
            return animals.Find(_data => _data.Name == _animalName);
        }
    }
}