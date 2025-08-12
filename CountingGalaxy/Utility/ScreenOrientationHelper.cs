using UnityEngine;

namespace Utility
{
    public static class ScreenOrientationHelper
    {
        public static void ForcePortrait()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        public static void ForceLandscape()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        public static void SetAutoPortrait()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        public static void SetAutoLandscape()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
}