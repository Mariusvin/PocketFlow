using UnityEditor;
using UnityEngine;
using Utility.CustomAttributes;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label) 
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) {

            GUI.enabled = false;
            EditorGUI.PropertyField(_position, _property, _label, true);
            GUI.enabled = true;
        }
    }
}