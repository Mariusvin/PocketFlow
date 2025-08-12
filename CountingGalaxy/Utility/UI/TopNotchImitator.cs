using TutoTOONS.Subscription.UI;
using UnityEngine;

namespace Utility.UI
{
    public class TopNotchImitator : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private SafeAreaHelper safeAreaHelper;

        private const float TOP_NOTCH_OFFSET = -40.0f;

        private void OnValidate()
        {
            if (!rectTransform)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (!safeAreaHelper)
            {
                safeAreaHelper = GetComponent<SafeAreaHelper>();
            }
        }

        private void Start()
        {
            TryApplyTopNotchOffset();
        }

        private void TryApplyTopNotchOffset()
        {
            float _topNotchSize = SafeAreaHelper.TopNotchSize();
            if (_topNotchSize > 0.0f)
            {
                return;
            }

            safeAreaHelper.enabled = false;
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, TOP_NOTCH_OFFSET);
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, Screen.safeArea.yMin);
        }
    }
}