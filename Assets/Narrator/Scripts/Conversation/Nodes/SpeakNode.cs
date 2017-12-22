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

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class SpeakNode : Node
    {
        public void CreateSpeakNode(int id, NarratorBrainSO brain)
        {
            type = Type.speak;
            charac = new Character();

            entryBox = new Rect(windowRect.x - 10.0f, windowRect.y + windowRect.height * 0.8f, 10.0f, 10.0f);

            contents = new List<Content>();
            Content content = new Content();
            content.Initialize(brain);
            contents.Add(content);
        }

        public void CreateSpeakNode(int _id, int _choicesCount, NarratorBrainSO brain)
        {
            type = _choicesCount > 1 ? Type.choice : Type.speak;
            charac = new Character();

            entryBox = new Rect(windowRect.x - 10.0f, windowRect.y + windowRect.height * 0.8f, 10.0f, 10.0f);

            contents = new List<Content>();

            Content content = new Content();
            content.Initialize(brain);
            contents.Add(content);

            content = new Content();
            content.Initialize(brain);
            contents.Add(content);
        }
    }

}
