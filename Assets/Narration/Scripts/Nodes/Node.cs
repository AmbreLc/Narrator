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

using UnityEditor;
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
        [SerializeField][HideInInspector] public Rect windowRect;
        [SerializeField] [HideInInspector] public Vector2 position;
        [SerializeField][HideInInspector] public Rect entryBox;
        [SerializeField][HideInInspector] public List<ExitBox> exitBoxes;

        [SerializeField] public Character charac;
        [SerializeField] public string speak;
        [SerializeField] public List<string> choices;

        public void DrawWindow()
        {
            if(type == Type.speak)
                speak = EditorGUILayout.TextArea(speak);
            else if(type == Type.choice)
            {
                for(int i = 0; i < choices.Count; i++)
                {
                    choices[i] = EditorGUILayout.TextArea(choices[i]);
                }
            }
        }

        public void DrawBox()
        {
            if (type != Type.entry)
            {
                entryBox = new Rect(windowRect.x - 10.0f, windowRect.y + windowRect.height * 0.2f, 10.0f, 10.0f);
                GUI.Box(entryBox, "");
            }

            if (type == Type.choice)
            {
                for (int i = 0; i < choices.Count; i++)
                {
                    exitBoxes[i].rect = new Rect(windowRect.x + windowRect.width, windowRect.y + windowRect.height * (0.25f + i * 0.2f), 10.0f, 10.0f);
                    GUI.Box(exitBoxes[i].rect, "");
                }
            }
            else
            {
                exitBoxes[0].rect = new Rect(windowRect.x + windowRect.width, windowRect.y + windowRect.height * 0.8f, 10.0f, 10.0f);
                GUI.Box(exitBoxes[0].rect, "");
            }
        }
    }

}
