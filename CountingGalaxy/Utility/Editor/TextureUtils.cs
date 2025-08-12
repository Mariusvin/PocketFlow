using System.Text.RegularExpressions;
using UnityEngine;

namespace Utility.Editor
{
    public static class TextureUtils
    {
        /// <summary>
        /// Resizes a texture from byte data to a specific height while maintaining aspect ratio.
        /// </summary>
        /// <param name="_bytes">The raw byte data of the original image.</param>
        /// <param name="_targetHeight">The desired height of the new image.</param>
        public static byte[] ResizeTexture(byte[] _bytes, int _targetHeight)
        {
            Texture2D _originalTex = new(2, 2);
            _originalTex.LoadImage(_bytes);

            float _ratio = (float)_originalTex.width / _originalTex.height;
            int _newWidth = Mathf.RoundToInt(_targetHeight * _ratio);
        
            RenderTexture _rt = RenderTexture.GetTemporary(_newWidth, _targetHeight);
            Graphics.Blit(_originalTex, _rt);
        
            RenderTexture.active = _rt;
            Texture2D _resizedTex = new(_newWidth, _targetHeight);
            _resizedTex.ReadPixels(new Rect(0, 0, _newWidth, _targetHeight), 0, 0);
            _resizedTex.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(_rt);
            Object.DestroyImmediate(_originalTex);

            return _resizedTex.EncodeToPNG();
        }

        /// <summary>
        /// Generates a short, filesystem-safe thumbnail name from the article title.
        /// Format: "First_20_Chars_Of_Title_1234x512.png"
        /// </summary>
        public static string GenerateThumbnailName(string _title, int _width, int _height)
        {
            if (string.IsNullOrEmpty(_title))
            {
                return "Untitled_Thumbnail.png";
            }

            string _sanitized = Regex.Replace(_title, @"[^a-zA-Z0-9\s]", "");
            int _length = Mathf.Min(_sanitized.Length, 20);
            string _shortTitle = _sanitized[.._length].Replace(' ', '_');

            return $"{_shortTitle}_{_width}x{_height}.png";
        }
    }
}
