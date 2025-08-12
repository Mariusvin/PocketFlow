using UnityEngine;
using SpineAnimations.CountingGalaxyUniverse;
using SpineAnimations;
using Cutscenes;

namespace Activities.CountingGalaxy.Cutscenes
{
    public class CountingGalaxyCutsceneObjectBase : CutsceneObjectBase, ISpineObjectSkinInjector
    {
        [SerializeField] protected UniverseWorldSpineController universeSpineController;

        protected override void OnEnable()
        {
            base.OnEnable();
            cutsceneCallbacks.OnPlaySpecialParticles += universeSpineController.TryPlayCandleParticles;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            cutsceneCallbacks.OnPlaySpecialParticles -= universeSpineController.TryPlayCandleParticles;
        }

        public override void Initialize()
        {
            universeSpineController.Initialize();
            base.Initialize();
        }

        public void SetSpineObjectSkin(string _skinName)
        {
            universeSpineController.SetSkin(_skinName);
        }
    }
}