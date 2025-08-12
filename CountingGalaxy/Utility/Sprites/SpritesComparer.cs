using System.Collections.Generic;
using UnityEngine;

namespace Utility.Sprites
{
    public static class SpritesComparer
    {
        private static readonly float ALPHA_IGNORE = 0.1f;

        /// <summary>
        /// Calculates the average color of a sprite. Ignores transparent pixels. Useful for setting particles, etc.
        /// </summary>
        public static Color CalculateAverageColor(Sprite _sprite)
        {
            Texture2D _tex = _sprite.texture;
            Rect _rect = _sprite.textureRect;
            Color[] _pixels = _tex.GetPixels(
                (int)_rect.x,
                (int)_rect.y,
                (int)_rect.width,
                (int)_rect.height);

            float _r = 0, _g = 0, _b = 0;
            foreach (Color _pixel in _pixels)
            {
                if (_pixel.a <= 0f)
                {
                    continue;
                }

                _r += _pixel.r;
                _g += _pixel.g;
                _b += _pixel.b;
            }

            int _count = _pixels.Length;
            return new Color(_r / _count, _g / _count, _b / _count);
        }

        public static List<Sprite> GetSimilarSpritesByColor(Color _targetColor, List<Sprite> _sprites, float _similarityThreshold = 0.5f)
        {
            List<Sprite> _result = new();
            foreach (Sprite _sprite in _sprites)
            {
                if (_sprite == null)
                {
                    continue;
                }

                Color _dominantColor = CalculateDominantColor(_sprite);
                float _similarity = CalculateColorSimilarity(_targetColor, _dominantColor);
                if (_similarity >= _similarityThreshold)
                {
                    _result.Add(_sprite);
                }
            }

            return _result;
        }

        private static Color CalculateDominantColor(Sprite _sprite)
        {
            Texture2D _texture = _sprite.texture;
            Rect _rect = _sprite.textureRect;
            Color[] _pixels = _texture.GetPixels(
                (int)_rect.x,
                (int)_rect.y,
                (int)_rect.width,
                (int)_rect.height);

            float _r = 0, _g = 0, _b = 0;
            int _count = 0;
            foreach (Color _pixel in _pixels)
            {
                if (_pixel.a > ALPHA_IGNORE) // Ignore nearly transparent pixels
                {
                    _r += _pixel.r;
                    _g += _pixel.g;
                    _b += _pixel.b;
                    _count++;
                }
            }

            if (_count == 0)
            {
                return Color.black; // Default if no visible pixels
            }
            return new Color(_r / _count, _g / _count, _b / _count);
        }

        private static float CalculateColorSimilarity(Color _color1, Color _color2)
        {
            float _rDiff = _color1.r - _color2.r;
            float _gDiff = _color1.g - _color2.g;
            float _bDiff = _color1.b - _color2.b;
            float _distance = Mathf.Sqrt(_rDiff * _rDiff + _gDiff * _gDiff + _bDiff * _bDiff);
            return 1f - Mathf.Clamp01(_distance / Mathf.Sqrt(3));
        }
    }
}
