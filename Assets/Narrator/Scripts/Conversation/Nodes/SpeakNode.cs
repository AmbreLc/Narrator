/* NARRATOR PACKAGE : SpeakNode.cs
 * Created by Ambre Lacour
 * 
 * A SpeakNode represents a line of dialogue in a conversation
 * 
 * A SpeakNode :
 *      - is linked to other(s) SpeakNode(s) through its Content member
 *      
 * You can create/Edit/delete SpeakNode in the narrator window
 * You can look over a conversation's SpeakNodes via scripting in game
 */

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class SpeakNode : Node
    {
        /// <summary>
        /// [EDITOR ONLY] Constructor : default values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="brain"></param>
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

        /// <summary>
        /// [EDITOR ONLY] Constructor : default values for a choice node (several contents)
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_choicesCount"></param>
        /// <param name="brain"></param>
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
