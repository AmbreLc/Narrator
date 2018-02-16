/* NARRATOR PACKAGE : Node.cs
 * Created by Ambre Lacour
 * 
 * A node in a conversation
 * 
 * A node :
 *      - is a dragable window in the Narrator Window  
 *      - has a type (entry, speak or choice node) according to its content
 *      - has a list of contents (lines of dialog)
 *      - has a character (who is speaking ?)
 * 
 * EntryNode and SpeakNode derived from Node and are used to build conversations trees
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
        /// <summary>
        /// Node types
        /// </summary>
        public enum Type
        {
            entry,
            speak,
            choice
        };

        /// <summary>
        /// Node's type
        /// </summary>
        [SerializeField] public Type type;

        /// <summary>
        /// [EDITOR ONLY] Rect: defines the node window in the conversation tree (in editor)
        /// </summary>
        [SerializeField] [HideInInspector] public Rect windowRect;

        /// <summary>
        /// [EDITOR ONLY] The node window's position in the conversation tree (in editor) 
        /// </summary>
        [SerializeField] [HideInInspector] public Vector2 position;

        /// <summary>
        /// Where other nodes link to the node
        /// </summary>
        [SerializeField] [HideInInspector] public Rect entryBox;

        /// <summary>
        /// List of contents (lines of dialog)
        /// </summary>
        [SerializeField] public List<Content> contents;

        /// <summary>
        /// Character of the node (who is speaking ?)
        /// </summary>
        [SerializeField] public Character charac;


#if UNITY_EDITOR

        /// <summary>
        /// [EDITOR ONLY] Called when a langage is deleted to delete all content refering to it
        /// </summary>
        /// <param name="_languageIndex"></param>
        public void DeleteLanguageContent(int _languageIndex)
        {
            for(int c = 0; c < contents.Count; c++)
            {
                contents[c].texts.RemoveAt(_languageIndex);
            }
        }

        /// <summary>
        /// [EDITOR ONLY] Draw the node and its content(s)
        /// </summary>
        /// <param name="_currentLangage"></param>
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

        /// <summary>
        /// [EDITOR ONLY] Draw the entry and exit boxes of the node
        /// </summary>
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
