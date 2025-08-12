using UnityEngine;

namespace Activities.Shared.CornerFluvsie
{
    public readonly struct CornerBubbleSlotData
    {
        public Sprite SlotSprite { get; }

        public string SlotText { get; }

        public float TextSize { get; }
        
        public bool TextAutoSize { get; }
        
        public bool ContainsData => SlotSprite || !string.IsNullOrEmpty(SlotText);
        
        public CornerBubbleSlotData(Sprite _slotSprite)
        {
            SlotSprite = _slotSprite;
            SlotText = string.Empty;
            TextSize = 0.0f;
            TextAutoSize = false;
        }

        public CornerBubbleSlotData(string _slotText)
        {
            SlotSprite = null;
            SlotText = _slotText;
            TextSize = 0.0f;
            TextAutoSize = true;
        }
        
        public CornerBubbleSlotData(string _slotText, float _textSize)
        {
            SlotSprite = null;
            SlotText = _slotText;
            TextSize = _textSize;
            TextAutoSize = false;
        }

        public CornerBubbleSlotData(Sprite _slotSprite, string _slotText, float _textSize, bool _autoSize = false)
        {
            SlotSprite = _slotSprite;
            SlotText = _slotText;
            TextSize = _textSize;
            TextAutoSize = _autoSize;
        }
    }
}