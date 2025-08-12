using System;
using System.Collections.Generic;
using UnityEngine;
using Activities.CountingGalaxy.ActivityParts;
using Activities.Shared.Architecture;
using Activities.Shared.Data;
using Cutscenes;

namespace Activities.CountingGalaxy
{
    public class CountingGalaxyActivityManager : MultiplePartsActivityController<BaseActivityData, ActivityPartBase<CountingGalaxyPartName>, CountingGalaxyPartName>
    {
        [Header("Scene References")]
        [SerializeField] private SpineCutscenesManager cutscenesManager;
        
        private CGActivityData convertedActivityData;
        private CountingGalaxyPartsController convertedPartsController;
        private List<ObjectSkinName> currentOrderOfCounting;
        
        protected override void Initialize()
        {
            base.Initialize();
            if (!TryConvertPartsController(out convertedPartsController))
            {
                Debug.LogError("Failed to convert CountingGalaxyPartsController!");
                EndActivityOnExit();
                return;
            }

            if (!TryConvertActivityData(out convertedActivityData))
            {
                Debug.LogError("Failed to convert CGActivityData!");
                EndActivityOnExit();
                return;
            }
            
            currentOrderOfCounting = convertedActivityData.GetNextOrderOfCounting();
            cutscenesManager.Initialize();
            cutscenesManager.SetCutsceneSpineObjectSkins(convertedActivityData.GetSkinNameForObject(currentOrderOfCounting[0]));
            foreach (ObjectSkinName _skinName in currentOrderOfCounting)
            {
                convertedPartsController.SpawnPart(convertedActivityData.GetVisualsForSkin(_skinName));
            }
            
            cutscenesManager.BeginNextCutscene(partsController.Begin);
            //partsController.Begin();
        }

        protected override void HandlePartStarted(CountingGalaxyPartName _partName, int _partIndex)
        {
            base.HandlePartStarted(_partName, _partIndex);
            if (_partName == CountingGalaxyPartName.Counting)
            {
                
            }
        }

        private void Continue()
        {
            cutscenesManager.BeginNextCutscene();
        }
    }
}