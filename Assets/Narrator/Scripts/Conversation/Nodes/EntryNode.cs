/* NARRATOR PACKAGE : EntryNode.cs
 * Created by Ambre Lacour
 * 
 * An EntryNode represents the begining of a conversation
 * 
 * An EntryNode :
 *      - is a dragable window in the Narrator Window  
 *      - links to other(s) SpeakNode(s)
 *      
 */

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class EntryNode : Node
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public void CreateEntryNode()
        {
            windowRect = new Rect(250.0f, 100.0f, 100.0f, 60.0f);

            charac = null;
            type = Type.entry;

            contents = new List<Content>();
            Content content = new Content();
            content.InitializeForEntryNode();
            contents.Add(content);
        }
    }

}
