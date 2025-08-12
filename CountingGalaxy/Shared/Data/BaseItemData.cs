using System;
using UnityEngine;
using Utility.CustomAttributes;

namespace Activities.Shared.Data
{
    public class BaseItemData<T> : ScriptableObject where T : Enum
    {
        [Header("Base Settings")]
        [SerializeField] private T itemType;
        [PreviewSprite][SerializeField] private Sprite itemSprite;

        public T ItemType => itemType;
        public Sprite ItemSprite => itemSprite;
    }
}