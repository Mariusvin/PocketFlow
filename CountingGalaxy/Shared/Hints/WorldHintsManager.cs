using UnityEngine;

namespace Activities.Shared.Hints
{
    public class WorldHintsManager : HintsManagerBase<WorldHintsManager>
    {
        protected override string HintObjectPrefabPath => "Assets/Prefabs/Shared/Hints/WorldHintObject.prefab";

        protected override void ApplyHintPosition<TPosition>(HintObject _hint, TPosition _position)
        {
            if (_position is not Vector3 _worldPosition)
            {
                Debug.LogError("Invalid position type passed to ApplyHintPosition");
                return;
            }

            _hint.Position = _worldPosition;
        }
    }
}