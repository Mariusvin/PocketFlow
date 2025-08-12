using UnityEngine;
using PrimeTween;
using TMPro;

namespace Activities.Shared
{
    public class InfoCard : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private TextMeshPro categoryText;

        private const float SCALE_DURATION_SECONDS = 0.2f;
        private const float SCALE_MULTIPLIER = 1.2f;

        private Tween scaleTween;

        public void Initialize(Sprite _cardIcon, string _category)
        {
            iconRenderer.sprite = _cardIcon;
            categoryText.text = _category;

            transform.localScale = Vector3.zero;
        }

        public void Reveal(bool _highlight = false)
        {
            Vector3 _targetScale = _highlight ? Vector3.one * SCALE_MULTIPLIER : Vector3.one;
            ScaleCard(_targetScale, SCALE_DURATION_SECONDS, Ease.OutBounce);
        }

        public void StandOut(bool _highlight)
        {
            if (_highlight)
            {
                return; // Don't stand out if card should be highlighted
            }

            ScaleCard(Vector3.one * SCALE_MULTIPLIER, SCALE_DURATION_SECONDS, Ease.OutBounce);
        }

        private void ScaleCard(Vector3 _targetScale, float _durationSeconds, Ease _ease)
        {
            scaleTween.Stop();
            scaleTween = Tween.Scale(transform, _targetScale, _durationSeconds, _ease);
        }
    }
}