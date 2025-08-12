using UnityEditor;
using UnityEngine;
using Utility.CustomAttributes;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class UnityTagAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, _label, _property);
            _property.stringValue = EditorGUI.TagField(_position, _label, _property.stringValue);
            EditorGUI.EndProperty();
        }
    }
}