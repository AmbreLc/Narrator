using UnityEngine;
using UnityEditor;
using System;

namespace Narrator
{
    [CustomEditor(typeof(NarratorBrainSO))]
    public class NarratorInspector_Brain : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

                if (GUILayout.Button("Edit brain"))
                {
                    NarratorWindow.ShowEditor();
                }
        }
    }

    [CustomEditor(typeof(ConversationSO))]
    [CanEditMultipleObjects]
    public class NarratorInspector_Conversation : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Edit conversation"))
            {
                NarratorWindow.ShowEditor();
            }
        }
    }
}
