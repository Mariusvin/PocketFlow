using System;
using UnityEditor;
using UnityEngine;
using Utility.CustomAttributes;

namespace Utility.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PreviewSpriteAttribute))]
    public class PreviewSpriteDrawer : PropertyDrawer
    {
        private const float IMAGE_HEIGHT = 100;

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            if (_property.propertyType == SerializedPropertyType.ObjectReference &&
                (_property.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(_property, _label, true) + IMAGE_HEIGHT + 10;
            }

            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }

        private static string GetPath(SerializedProperty _property)
        {
            string path = _property.propertyPath;
            int index = path.LastIndexOf(".", StringComparison.Ordinal);
            return path.Substring(0, index + 1);
        }

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            //Draw the normal property field
            EditorGUI.PropertyField(_position, _property, _label, true);

            if (_property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Sprite _sprite = _property.objectReferenceValue as Sprite;
                if (_sprite != null)
                {
                    _position.y += EditorGUI.GetPropertyHeight(_property, _label, true) + 5;
                    _position.height = IMAGE_HEIGHT;

                    //GUI.DrawTexture(position, sprite.texture, ScaleMode.ScaleToFit);
                    DrawTexturePreview(_position, _sprite);
                }
            }
        }

        private void DrawTexturePreview(Rect _position, Sprite _sprite)
        {
            Vector2 _fullSize = new(_sprite.texture.width, _sprite.texture.height);
            Vector2 _size = new(_sprite.textureRect.width, _sprite.textureRect.height);

            Rect _coords = _sprite.textureRect;
            _coords.x /= _fullSize.x;
            _coords.width /= _fullSize.x;
            _coords.y /= _fullSize.y;
            _coords.height /= _fullSize.y;

            Vector2 _ratio;
            _ratio.x = _position.width / _size.x;
            _ratio.y = _position.height / _size.y;
            float _minRatio = Mathf.Min(_ratio.x, _ratio.y);

            Vector2 _center = _position.center;
            _position.width = _size.x * _minRatio;
            _position.height = _size.y * _minRatio;
            _position.center = _center;

            GUI.DrawTextureWithTexCoords(_position, _sprite.texture, _coords);
        }
    }
#endif

}
