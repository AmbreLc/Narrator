/* NARRATOR PACKAGE: Content.cs
 * Created by Ambre Lacour
 * 
 * A content is a dialog line in a node (a node can have more than 1 content)
 * 
 * A content :
 *      - has a list of texts (its dialog line in all the langages)
 *      - has an exit box to link to other nodes (in the narrator window)
 *      - has a list of next nodes (the nodes it links to)
 *      
 * You can create/edit/remove content on nodes in the narrator window
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
        //__________VARIABLES__________//

        /// <summary>
        /// Lines of dialogs in different langages
        /// </summary>
        public List<string> texts;

        /// <summary>
        /// [EDITOR] Box to link the line to other nodes
        /// </summary>
        public Rect exitBox;

        /// <summary>
        /// Nodes the content is linking to
        /// </summary>
        public List<NextNode> nextNodes;



        //___________METHODS__________//

        /// <summary>
        /// Create a new Content with default values
        /// </summary>
        /// <param name="brain"></param>
        public void Initialize(NarratorBrainSO brain)
        {
            texts = new List<string>();
            for (int i = 0; i < brain.Langages.Count; i++)
            {
                texts.Add("What should I say ?");
            }

            nextNodes = new List<NextNode>();
            exitBox = new Rect();
        }

        /// <summary>
        /// Create a new Content with entry node values
        /// </summary>
        public void InitializeForEntryNode()
        {
            nextNodes = new List<NextNode>();
            exitBox = new Rect();
        }

        /// <summary>
        /// Add a next node
        /// </summary>
        /// <param name="_index"></param>
        public void AddNextNode(int _index)
        {
            NextNode node = new NextNode();
            node.index = _index;
            
            nextNodes.Add(node);
        }

        /// <summary>
        /// Remove a next node
        /// </summary>
        /// <param name="_index"></param>
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
