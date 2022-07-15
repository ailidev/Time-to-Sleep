using UnityEngine;
using UnityEditor;

namespace Aili.CoreEditor
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PlayerController player = (PlayerController)target;

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("REFERENCES");
                player.m_Animator = (Animator)EditorGUILayout.ObjectField("Animator", player.m_Animator, typeof(Animator), true);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
