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

            charac = null;
            speak = string.Empty;
            type = Type.entry;
            choices = new List<string>();
            choices.Clear();
            choices.Add("My choice");

            entryBox = new Rect(windowRect.x - 10.0f, windowRect.y + windowRect.height * 0.2f, 10.0f, 10.0f);
            exitBoxes = new List<ExitBox>();
            ExitBox box = new ExitBox();
            box.Initialize(new Rect(windowRect.x + windowRect.width, windowRect.y + windowRect.height * 0.8f, 10.0f, 10.0f));
            exitBoxes.Add(box);
        }
    }

}
