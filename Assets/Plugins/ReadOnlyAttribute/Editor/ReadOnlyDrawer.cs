#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Aili.ReadOnlyField
{
    /// <summary>
    /// Custom Drawer for ReadOnlyAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Method for drawing GUI in Editor
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="property">Property</param>
        /// <param name="label">Label</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Save previous GUI value
            bool m_PreviousGUIState = GUI.enabled;

            // Disable edit for property
            GUI.enabled = false;

            // Draw property
            EditorGUI.PropertyField(position, property, label);

            // Set old GUI previous state
            GUI.enabled = m_PreviousGUIState;
        }
    }
}
#endif
