using System.Collections.Generic;
using System;
using UnityEngine;
using Utility.CustomAttributes;
using Utility;

namespace Activities.Shared.Letters
{
    [CreateAssetMenu(menuName = "Activities/Shared/Letters/Letters Dataset")]
    public class LettersDataset : ScriptableObject
    {
        [SerializeField] private List<LetterData> letters;

        private Dictionary<LetterName, LetterData> mappedLettersData;
        private Dictionary<char, LetterName> mappedLetters;

        public List<Sprite> GetWordSprites(string _word)
        {
            if (mappedLettersData == null)
            {
                MapLettersData();
            }

            if (mappedLetters == null)
            {
                MapLetters();
            }

            List<Sprite> _wordSprites = new List<Sprite>();
            foreach (char _letter in _word.ToUpperInvariant()) // Latin letters only
            {
                if (!mappedLetters.TryGetValue(_letter, out LetterName _letterName))
                {
                    Debug.LogError($"Failed to get LetterName from '{_letter}' in word: {_word}");
                    continue;
                }

                if (!mappedLettersData.TryGetValue(_letterName, out LetterData _letterData))
                {
                    Debug.LogError($"Failed to find LetterData for letter: {_letterName}");
                    continue;
                }

                _wordSprites.Add(_letterData.Sprite);
            }

            return _wordSprites;
        }

        private void MapLettersData()
        {
            mappedLettersData = new Dictionary<LetterName, LetterData>();
            foreach (LetterData _letter in letters)
            {
                mappedLettersData.TryAdd(_letter.Letter, _letter);
            }
        }

        private void MapLetters()
        {
            mappedLetters = new Dictionary<char, LetterName>();
            foreach (LetterData _letter in letters)
            {
                mappedLetters.TryAdd(_letter.Letter.ToString().ToUpperInvariant()[0], _letter.Letter); // Latin letters only
            }
        }

        [Serializable]
        private struct LetterData : ISerializationCallbackReceiver
        {
            [HideInInspector][SerializeField] private string name; // For inspector display only

            [field: SerializeField] public LetterName Letter { get; private set; }
            [field: SerializeField, PreviewSprite] public Sprite Sprite { get; private set; }

            public void OnBeforeSerialize()
            {
                SetName();
            }

            public void OnAfterDeserialize() { }

            private void SetName()
            {
                name = Letter.ToString();
            }
        }
    }
}