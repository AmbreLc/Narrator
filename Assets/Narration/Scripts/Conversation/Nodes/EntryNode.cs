/* NARRATOR PACKAGE
 * EntryNode.cs
 * Created by Ambre Lacour, 22/10/2017
 * Editor script used by NarrationEditor in the NarratorWindow
 * 
 * An EntryNode represents the begining of a conversation, it:
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

        public void CreateEntryNode()
        {
            windowRect = new Rect(250.0f, 100.0f, 100.0f, 60.0f);

            id = -1;
            charac = null;
            type = Type.entry;

            contents = new List<Content>();
            Content content = new Content();
            content.Initialize();
            contents.Add(content);
        }
    }

}
