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
    public class SpeakNode : Node
    {
        public void CreateSpeakNode()
        {
            speak = "What sould I say ?";
            charac = new Character();
            type = Type.speak;
            choices = new List<string>();
            choices.Add("My choice");

            entryBox = new Rect(windowRect.x - 10.0f, windowRect.y + windowRect.height * 0.8f, 10.0f, 10.0f);
            exitBoxes = new List<ExitBox>();
            ExitBox box = new ExitBox();
            box.Initialize(new Rect(windowRect.x + windowRect.width, windowRect.y + windowRect.height * 0.8f, 10.0f, 10.0f));
            exitBoxes.Add(box);
        }
    }

}
