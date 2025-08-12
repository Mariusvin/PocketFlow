using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Utility.Extensions
{
    // Only way to get the real screen size in the editor
    public static class ScreenExtensions
    {
        public static float Width
        {
            get
            {
                float _screenWidth = Screen.width;
#if UNITY_EDITOR
                string[] _res = UnityStats.screenRes.Split('x');
                _screenWidth = int.Parse(_res[0]);
#endif
                return _screenWidth;
            }
        }
        
        public static float Height
        {
            get
            {
                float _screenHeight = Screen.height;
#if UNITY_EDITOR
                string[] _res = UnityStats.screenRes.Split('x');
                _screenHeight = int.Parse(_res[1]);
#endif
                return _screenHeight;
            }
        }
    }
}
