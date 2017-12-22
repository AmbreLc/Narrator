/* NARRATOR PACKAGE
 * SpeakNode.cs
 * Created by Ambre Lacour, 12/10/2017
 * Editor script used by NarrationEditor in the NarratorWindow
 * 
 * A SpeakNode represents a line of dialogue in a conversatio, it:
 *      - is a dragable window in the Narrator Window  
 *      - is linked to other(s) SpeakNode(s) through its speak member (see the Speak class to learn more)
 *      
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class Node
    {
        public enum Type
        {
            entry,
            speak,
            choice
        };

        [SerializeField] public Type type;
        [SerializeField] [HideInInspector] public Rect windowRect;
        [SerializeField] [HideInInspector] public Vector2 position;
        [SerializeField] [HideInInspector] public Rect entryBox;
        [SerializeField] public List<Content> contents;

        [SerializeField] public Character charac;

#if UNITY_EDITOR
        public void DeleteLanguageContent(int _languageIndex)
        {
            for(int c = 0; c < contents.Count; c++)
            {
                contents[c].texts.RemoveAt(_languageIndex);
            }
        }
        public void DrawWindow(int _currentLangage)
        {
            if (contents.Count > 1)
                type = Type.choice;

            for (int i = 0; i < contents.Count; i++)
            {
                if (type != Type.entry)
                {
                    if (contents[i].texts.Count <= _currentLangage)
                        contents[i].texts.Add("What should I say ?");
                    contents[i].texts[_currentLangage] = EditorGUILayout.TextArea(contents[i].texts[_currentLangage]);
                }
            }
        }
        public void DrawBox()
        {
            if (type != Type.entry)
            {
                entryBox = new Rect(windowRect.x - 10.0f, windowRect.y + windowRect.height * 0.2f, 10.0f, 10.0f);
                GUI.Box(entryBox, "");

                for (int i = 0; i < contents.Count; i++)
                {
                    contents[i].exitBox = new Rect(windowRect.x + windowRect.width, windowRect.y + windowRect.height * (0.25f + i * 0.2f), 10.0f, 10.0f);
                    GUI.Box(contents[i].exitBox, "");
                }
            }

            else
            {
                contents[0].exitBox = new Rect(windowRect.x + windowRect.width, windowRect.y + windowRect.height * 0.5f, 10.0f, 10.0f);
                GUI.Box(contents[0].exitBox, "");
            }
        }
#endif

    }
  
}
