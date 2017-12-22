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
    public class Content
    {
        public List<string> texts;
        public Rect exitBox;
        public List<NextNode> nextNodes;

        public void Initialize(NarratorBrainSO brain)
        {
            texts = new List<string>();
            for (int i = 0; i < brain.Languages.Count; i++)
            {
                texts.Add("What should I say ?");
            }

            nextNodes = new List<NextNode>();
            exitBox = new Rect();
        }

        public void InitializeForEntryNode()
        {
            nextNodes = new List<NextNode>();
            exitBox = new Rect();
        }

        public void AddNextNode(int _index)
        {
            NextNode node = new NextNode();
            node.index = _index;
            
            nextNodes.Add(node);
        }

        public void RemoveNextNode(int _index)
        {
            bool hasRemoved = false;
            for (int i = 0; i < nextNodes.Count; i++)
            {
                if (nextNodes[i].index == _index)
                {
                    nextNodes.RemoveAt(i);
                    hasRemoved = true;
                }
            }
            if (hasRemoved == false)
                Debug.LogError("Failed to find end node link, deletion won't be effective.");
        }

    }
}
