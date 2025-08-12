using System;
using System.Collections.Generic;
using System.Linq;
using Activities.Shared.Data;
using UnityEngine;
using Utility.Extensions;
using Random = UnityEngine.Random;

namespace Activities.CountingGalaxy
{
    public class CGActivityData : BaseActivityData
    {
        [Header("--- Counting Galaxy ---")]
        [Header("Short Game Settings")] 
        [SerializeField] private int minNumberOfCountedItems = 3;
        [SerializeField] private int maxNumberOfCountedItems = 9;

        [Header("Extended Game Settings")] 
        [SerializeField] private List<OrderOfCounting> possibleOrdering;

        [Header("References")]
        [SerializeField] private List<ObjectVisuals> objectVisuals;
        
        private static int usedOrderIndex;
        
        public int GenerateRandomCountedItemsNumber => Random.Range(minNumberOfCountedItems, maxNumberOfCountedItems + 1);
        
        public List<ObjectSkinName> GetNextOrderOfCounting()
        {
            if (possibleOrdering.Count == 0)
            {
                Debug.LogError("No possible ordering defined for Counting Galaxy activity!");
                return new List<ObjectSkinName>();
            }

            if (usedOrderIndex == 0) // Shuffle when the previous sequence is completed
            {
                possibleOrdering.Shuffle();
            }

            usedOrderIndex = (usedOrderIndex + 1) % possibleOrdering.Count;
            return possibleOrdering[usedOrderIndex].Order;
        }
        
        public ObjectVisuals GetVisualsForSkin(ObjectSkinName _skinName)
        {
            if (objectVisuals.Count <= 0)
            {
                Debug.LogError("Object visuals are not defined in CGActivityData!");
                return new ObjectVisuals();
            }
            
            int _index = objectVisuals.FindIndex(_v => _v.SkinNameEnum == _skinName);
            if (_index < 0)
            {
                Debug.LogError($"{_skinName} is not defined. Available skins: {string.Join(", ", objectVisuals.Select(_v => _v.SkinNameEnum))}");
                return objectVisuals[0];
            }

            return objectVisuals[_index];
        }

        public string GetSkinNameForObject(ObjectSkinName _skinName)
        {
            int _index = objectVisuals.FindIndex(_v => _v.SkinNameEnum == _skinName);
            if (_index < 0)
            {
                Debug.LogError($"{_skinName} is not defined. Available skins: {string.Join(", ", objectVisuals.Select(_v => _v.SkinNameEnum))}");
                return string.Empty;
            }

            return objectVisuals[_index].SkinName;
        }
        
        [Serializable]
        private struct OrderOfCounting
        {
            [field: SerializeField] public List<ObjectSkinName> Order { get; private set; }
        }
    }
}
