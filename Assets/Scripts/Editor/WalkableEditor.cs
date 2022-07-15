using UnityEngine;
using UnityEditor;

namespace Aili.CoreEditor
{
    [CustomEditor(typeof(Walkable))]
    public class WalkableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
        }
    }
}
