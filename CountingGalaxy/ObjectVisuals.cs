using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Utility.CustomAttributes;

namespace Activities.CountingGalaxy
{
    [Serializable]
    public struct ObjectVisuals
    {
        #if UNITY_EDITOR
        [SerializeField] private SkeletonDataAsset skeleton;
        #endif
        [field: SpineSkin(dataField: nameof(skeleton))] [field: SerializeField] public string SkinName { get; private set; } // Center object
        [field: SerializeField] public ObjectSkinName SkinNameEnum { get; private set; }
        [field: PreviewSprite] [field: SerializeField] public Sprite CenterObjectSprite { get; private set; }
        [field: PreviewSprite] [field: SerializeField] public Sprite CenterObjectShine { get; private set; }
        [field: PreviewSprite] [field: SerializeField] public List<Sprite> PossibleSprites { get; private set; } // Surrounding objects
    }
}
