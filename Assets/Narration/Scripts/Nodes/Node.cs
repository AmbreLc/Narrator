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
        public void DrawWindow()
        {
            if (contents.Count > 1)
                type = Type.choice;

            for (int i = 0; i < contents.Count; i++)
            {
                if (type != Type.entry)
                    contents[i].text = EditorGUILayout.TextArea(contents[i].text);
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


    [System.Serializable]
    public class Content
    {
        public string text;
        public Rect exitBox;
        public List<NextNode> nextNodes;

        public void Initialize()
        {
            text = "";
            nextNodes = new List<NextNode>();
            exitBox = new Rect();
        }

        public void Initialize(Rect _rect)
        {
            nextNodes = new List<NextNode>();
            exitBox = new Rect(_rect);
        }

        public void AddNextNode(int _index)
        {
            NextNode node = new NextNode();
            node.index = _index;
            node.conditions = new List<Condition>();
            node.impacts = new List<Impact>();
            nextNodes.Add(node);
        }

        public void RemoveNextNode(int _index)
        {
            if (nextNodes.Count >= _index)
                nextNodes.RemoveAt(_index);
            else
                Debug.LogError("Can't remove next node: current node isn't linked to it");
        }
    }


    [System.Serializable]
    public class NextNode
    {
        public int index;
        public List<Condition> conditions;
        public List<Impact> impacts;
    }

    [System.Serializable]
    public class Condition
    {
        public string name;
        public Parameters.TYPE type;
        public Parameters.OPERATOR test;
        public int intMarker;
        public float floatMarker;
        public bool boolMarker;


        public bool IsComplete(Parameters _params)
        {
            if (_params == null)
            {
                Debug.LogError("Cannot test condition, parameters are null object");
                return false;
            }

            switch (type)
            {
                case Parameters.TYPE.f:
                    return _params.TestFloat(name, test, floatMarker);
                case Parameters.TYPE.b:
                    return _params.TestBool(name, test, boolMarker);
                case Parameters.TYPE.i:
                    return _params.TestInt(name, test, intMarker);
                default:
                    Debug.LogError("Cannot test condition, type is unknown");
                    return false;
            }
        }

    }


    [System.Serializable]
    public class Impact
    {
        public string name;
        public Parameters.TYPE type;
        public int intModifier;
        public float floatModifier;
        public bool boolModifier;

        public void ApplyImpact(Parameters _params)
        {
            switch (type)
            {
                case Parameters.TYPE.f:
                    float fValue = _params.GetFloat(name);
                    _params.SetFloat(name, fValue + floatModifier);
                    break;
                case Parameters.TYPE.b:
                    _params.SetBool(name, boolModifier);
                    break;
                case Parameters.TYPE.i:
                    int iValue = _params.GetInt(name);
                    _params.SetInt(name, iValue + intModifier);
                    break;
                default:
                    Debug.LogError("Cannot test condition, type is unknown");
                    break;
            }
        }
    }

}
