using UnityEngine;
using TMPro;

namespace Utility.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class AdjustRectToText : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI textRef;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float minHeight = 200f;
        [SerializeField] private float maxHeight = 1000f;
        [SerializeField] private float padding = 10f;
        
        private void OnValidate()
        {
            if(!textRef)
            {
                textRef = GetComponent<TextMeshProUGUI>();
            }
            
            if(!rectTransform)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            OnTextChanged("");
        }

        private void OnEnable()
        {
            inputField.onValueChanged.AddListener(OnTextChanged);
        }

        private void OnDisable()
        {
            inputField.onValueChanged.RemoveListener(OnTextChanged);
        }

        private void OnTextChanged(string _)
        {
            if (textRef.preferredHeight > maxHeight)
            {
                textRef.autoSizeTextContainer = true;
                return;
            }

            textRef.autoSizeTextContainer = false;
            float _newHeight = Mathf.Clamp(textRef.preferredHeight + padding, minHeight, maxHeight);
            Vector2 _curSize = rectTransform.sizeDelta;
            rectTransform.sizeDelta = new Vector2(_curSize.x, _newHeight);
        }
    }
}
