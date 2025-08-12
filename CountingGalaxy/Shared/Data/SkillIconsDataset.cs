using System.Collections.Generic;
using System;
using UnityEngine;
using Activities.Shared.Architecture;
using Utility.CustomAttributes;

namespace Activities.Shared.Data
{
    [CreateAssetMenu(menuName = "Activities/Skill Icons Dataset")]
    public class SkillIconsDataset : ScriptableObject
    {
        [SerializeField] private List<SkillIconData> skillIcons;

        private Dictionary<SkillCategoryName, Sprite> mappedSkillIcons;

        public Sprite TryGetSkillIcon(SkillCategoryName _skillCategory)
        {
            if (mappedSkillIcons == null)
            {
                MapSkillIcons();
            }

            if (mappedSkillIcons.TryGetValue(_skillCategory, out Sprite _icon))
            {
                return _icon;
            }

            return mappedSkillIcons.TryGetValue(SkillCategoryName.None, out Sprite _defaultIcon) ? _defaultIcon : null;
        }

        private void MapSkillIcons()
        {
            mappedSkillIcons = new Dictionary<SkillCategoryName, Sprite>();
            foreach (SkillIconData _iconData in skillIcons)
            {
                mappedSkillIcons.TryAdd(_iconData.SkillCategory, _iconData.SkillIcon);
            }
        }

        [Serializable]
        private struct SkillIconData
        {
            [field: SerializeField] public SkillCategoryName SkillCategory { get; private set; }
            [field: SerializeField, PreviewSprite] public Sprite SkillIcon { get; private set; }
        }
    }
}