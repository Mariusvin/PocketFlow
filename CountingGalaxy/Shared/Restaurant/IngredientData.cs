using UnityEngine;
using Utility.CustomAttributes;

namespace Activities.Shared.Restaurant
{
    [CreateAssetMenu(fileName = "IngredientData", menuName = "Activities/Restaurant/IngredientData")]
    public class IngredientData : ScriptableObject
    {
        [field: SerializeField] public IngredientName IngredientName { get; private set; }
        
        [field: SerializeField] public Color InactiveColor { get; private set; } = Color.white; // Color when the ingredient can NOT be selected (used to fill backgrounds)
        
        [field: SerializeField] public Color BgColor { get; private set; } = Color.white; // The accent color for the background
        
        [SerializeField] [PreviewSprite] private Sprite ingredientSprite;

        public Sprite IngredientSprite => ingredientSprite;
    }
}
