using UnityEditor;
using UnityEngine;

namespace Todoist
{
    [CustomPropertyDrawer(typeof(PasswordAttribute))]
    public class PasswordAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
    }
}