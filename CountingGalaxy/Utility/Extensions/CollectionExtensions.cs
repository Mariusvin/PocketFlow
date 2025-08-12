using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly System.Random RNG = new();
    
        public static T GetRandomItemFromList<T>(this IReadOnlyList<T> _list)
        {
            return _list.Count > 0 ? _list[RNG.Next(_list.Count)] : default;
        }

        public static void Shuffle<T>(this IList<T> _list)
        {
            if (_list == null)
            {
                throw new ArgumentNullException(nameof(_list), "List cannot be null.");
            }

            if (_list.Count < 2)
            {
                return;
            }
            
            int n = _list.Count;
            while (n > 1)
            {
                n--;
                int k = RNG.Next(n + 1);
                (_list[k], _list[n]) = (_list[n], _list[k]);
            }
        }
        
        public static bool ShuffleNoRepeats<T>(this List<T> _list)
        {
            if (_list == null)
            {
                throw new ArgumentNullException(nameof(_list), "List cannot be null.");
            }

            if (_list.Count < 2)
            {
                return true;
            }

            var _groupedValues = _list.GroupBy(_x => _x).Select(_g => new { Value = _g.Key, Count = _g.Count() }).ToList();

            // Check if the most frequent value appears more than half the time (which would make shuffling without repeats impossible)
            if (_groupedValues.Any(g => g.Count > (_list.Count + 1) / 2))
            {
                return false; // It's impossible to shuffle without consecutive duplicates
            }

            Random _rng = new();
            for (int i = _list.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_list[i], _list[j]) = (_list[j], _list[i]);
            }

            for (int i = 1; i < _list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_list[i], _list[i - 1]))
                {
                    for (int j = i + 1; j < _list.Count; j++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(_list[j], _list[i - 1]))
                        {
                            (_list[i], _list[j]) = (_list[j], _list[i]);
                            break;
                        }
                    }
                }
            }

            return true;
        }
    
        public static T[] GetValuesFromIndexCount<T>(this T[] _array, int _start, int _count)
        {
            T[] _newArray = new T[_count];
            int _dataIndex = 0;
            int _endIndex = _start + _count;
            for (int _i = _start; _i < _endIndex; _i++)
            {
                _newArray[_dataIndex] = _array[_i];
                _dataIndex++;
            }

            return _newArray;
        }

        public static IEnumerable<TV> AppendItemsFromDictionary<TK, TV>(this IEnumerable<TV> _destinationCollection, IReadOnlyDictionary<TK, IReadOnlyList<TV>> _dictionary, IReadOnlyList<TK> _keys)
        {
            foreach (TK _key in _keys)
            {
                if (_dictionary.TryGetValue(_key, out IReadOnlyList<TV> _value))
                {
                    _destinationCollection = _destinationCollection.Concat(_value);
                }
            }
            return _destinationCollection;
        }
        
        public static bool AreEqual<T>(this List<T> _list)
        {
            if (_list == null || _list.Count == 0)
            {
                return true;
            }
        
            T firstValue = _list[0];
            foreach (T item in _list)
            {
                if (!EqualityComparer<T>.Default.Equals(item, firstValue))
                {
                    return false; 
                }
            }
            return true;
        }
        
        public static void AddMany<T>(this List<T> _list, T _value, int _count)
        {
            if (_list == null)
            {
                throw new ArgumentNullException(nameof(_list), "List cannot be null.");
            }

            if (_count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_count), "Count cannot be negative.");
            }

            for (int i = 0; i < _count; i++)
            {
                _list.Add(_value);
            }
        }
    }
}
