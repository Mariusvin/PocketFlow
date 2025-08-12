using UnityEngine;
using Spine.Unity;

namespace Activities.Shared.JumperAnimal
{
    [CreateAssetMenu(menuName = "Activities/Shared/JumperAnimal/Jumper Animal Data")]
    public class AnimalData : ScriptableObject
    {
        [field: SerializeField] public AnimalName Name { get; private set; }
        [Space]

        [SerializeField] private SkeletonDataAsset skeletonData;
        [SpineSkin(dataField: nameof(skeletonData))][SerializeField] private string skinPath;

        public string SkinPath => skinPath;
    }
}