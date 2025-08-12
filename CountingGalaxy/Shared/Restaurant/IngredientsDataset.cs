using System.Collections.Generic;
using UnityEngine;

namespace Activities.Shared.Restaurant
{
    [CreateAssetMenu(fileName = "IngredientsDataset", menuName = "Activities/Restaurant/IngredientsDataset")]
    public class IngredientsDataset : ScriptableObject
    {
        [field: SerializeField] public IngredientData[] Ingredients { get; set; }
        
        public int IngredientsCount => Ingredients.Length;

        public IngredientData GetIngredientData(IngredientName _ingredientName)
        {
            foreach (IngredientData _ingredient in Ingredients)
            {
                if (_ingredient.IngredientName == _ingredientName)
                {
                    return _ingredient;
                }
            }

            return null;
        }
        
        public List<IngredientData> GetRandomIngredients(int _count)
        {
            if(_count > Ingredients.Length)
            {
                Debug.LogError("Not enough ingredients in the dataset!");
                return new List<IngredientData>();
            }
            
            List<IngredientData> _randomIngredients = new(_count);
            List<IngredientData> _ingredientsCopy = new(Ingredients);
            
            for (int i = 0; i < _count; i++)
            {
                int _rndIndex = Random.Range(0, _ingredientsCopy.Count);
                _randomIngredients.Add(_ingredientsCopy[_rndIndex]);
                _ingredientsCopy.RemoveAt(_rndIndex);
            }

            return _randomIngredients;
        }
    }
}
