using Activities.Shared.Architecture;
using UnityEngine;

namespace Activities.CountingGalaxy.ActivityParts
{
    public class CountingGalaxyPartsController : BasePartsController<ActivityPartBase<CountingGalaxyPartName>, CountingGalaxyPartName>
    {
        [SerializeField] private CountingPart countingPartPrefab;

        public void SpawnPart(ObjectVisuals _objectVisuals)
        {
            CountingPart _part = Instantiate(countingPartPrefab, transform);
            _part.SetInitialData(10, _objectVisuals.CenterObjectSprite, _objectVisuals.CenterObjectShine, _objectVisuals.PossibleSprites);
            _part.PartObjectEnabled = false;
            activityParts.Add(_part);
        }
    }
}