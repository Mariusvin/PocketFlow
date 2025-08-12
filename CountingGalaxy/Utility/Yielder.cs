using UnityEngine;
using System.Collections.Generic;

namespace Utility
{
    public static class Yielder
    {
        private static readonly Dictionary<float, WaitForSeconds> TIME_INTERVAL = new(100);

        public static WaitForEndOfFrame EndOfFrame { get; } = new();

        public static WaitForSeconds Wait(float _seconds)
        {
            if (!TIME_INTERVAL.ContainsKey(_seconds))
            {
                TIME_INTERVAL.Add(_seconds, new WaitForSeconds(_seconds));
            }

            return TIME_INTERVAL[_seconds];
        }
    }
}