/* NARRATOR PACKAGE : NextNode.cs
 * Created by Ambre Lacour
 * 
 * Informations about the node another node is linking to
 * 
 * A NextNode :
 *      - has an index, refering the Node index in the dialogs list of the conversation
 *      - has a list of conditions that enable or disable the transition
 *      - has a list of impact (consequences of the transition)
 *    
 * When a conversation tries to reach a NextNode in game, parameters are tested and modified according to its conditions and impacts
 * The NextNode editing is made via the Link class in the narrator window.
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class NextNode
    {
        /// <summary>
        /// Index of the next dialog in the dialogs list of the conversation
        /// </summary>
        public int index;

        /// <summary>
        /// List of conditions, next node will be reached only if they are all completed
        /// </summary>
        public List<Condition> conditions;

        /// <summary>
        /// List of conditions, will modify parameters if next node is reached
        /// </summary>
        public List<Impact> impacts;


        /// <summary>
        /// Constructor
        /// </summary>
        public NextNode()
        {
            conditions = new List<Condition>();
            impacts = new List<Impact>();
        }
    }
}
