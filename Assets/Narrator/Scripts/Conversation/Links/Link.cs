/* NARRATOR PACKAGE : Link.cs
 * Created by Ambre Lacour
 * 
 * A link connect two dialogs node in the narrator window (editor class only)
 * 
 * A link:
 *      - has a starting node and an ending node
 *      - contains conditions and impacts (paramaters to test or modify)
 *      - has a Rect to display conditions and impacts in the narrator window
 *      
 * You can create/edit/delete a link using the narrator window
 * In game, the links informations are available in the NextNode class
 * 
 */

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace Narrator
{
    public class Link
    {
        //__________VARIABLES___________//

        /// <summary>
        /// Dialog node from where we are linking
        /// </summary>
        public Node start;

        /// <summary>
        /// Dialog node to where we are linking
        /// </summary>
        public Node end;


        /// <summary>
        /// In the start node, which content we are linking from
        /// </summary>
        public int startBoxIndex = 0;

        /// <summary>
        /// In the end node, which content we are linking to
        /// </summary>
        public int nextNodeIndex = 0;


        /// <summary>
        /// All link's conditions
        /// </summary>
        public List<Condition> conditions;

        /// <summary>
        /// All link's impacts
        /// </summary>
        public List<Impact> impacts;


        /// <summary>
        /// [EDITOR ONLY] Rect used to display impacts & conditions in the narrator window
        /// </summary>
        public Rect linkRect;

        /// <summary>
        /// [EDITOR ONLY] Link's color in the narrator window
        /// </summary>
        public Color color;

        /// <summary>
        /// [EDITOR ONLY] Is the user editing the link in the narrator window
        /// </summary>
        public bool isMoving;




        //___________METHODS_________//

        /// <summary>
        /// Default constructor
        /// </summary>
        public Link()
        {
            start = null;
            end = null;
            color = Color.white;
            isMoving = true;

            linkRect = new Rect(0.0f, 0.0f, 20.0f, 20.0f);
            conditions = new List<Condition>();
        }

        /// <summary>
        /// Constructor (when creating a link with no ending node)
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_startIndex"></param>
        /// <param name="_color"></param>
        public Link(Node _start, int _startIndex, Color _color)
        {
            start = _start;
            end = null;
            color = _color;
            isMoving = true;
            startBoxIndex = (short)_startIndex;

            nextNodeIndex = _start.contents[_startIndex].nextNodes.Count;

            linkRect = new Rect(0.0f, 0.0f, 20.0f, 20.0f);
            conditions = new List<Condition>();
            impacts = new List<Impact>();
        }

        /// <summary>
        /// Constructor (to create a link whith start node and end node)
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_startIndex"></param>
        /// <param name="_end"></param>
        /// <param name="_color"></param>
        public Link(Node _start, int _startIndex, Node _end, Color _color) 
        {
            start = _start;
            end = _end;
            color = _color;
            isMoving = false;
            startBoxIndex = (short)_startIndex;

            linkRect = new Rect(0.0f, 0.0f, 20.0f, 20.0f);
            Vector3 startPos = new Vector3(start.contents[startBoxIndex].exitBox.x + start.contents[startBoxIndex].exitBox.width / 2, start.contents[startBoxIndex].exitBox.y + start.contents[startBoxIndex].exitBox.height / 2, 0);
            Vector3 endPos = new Vector3(end.entryBox.x + end.entryBox.width / 2, end.entryBox.y + end.entryBox.height / 2, 0);         
            linkRect.x = (startPos.x + endPos.x) * 0.5f - linkRect.width * 0.5f;
            linkRect.y = (startPos.y + endPos.y) * 0.5f - linkRect.height * 0.5f;

            conditions = new List<Condition>();
            impacts = new List<Impact>();
        }

        /// <summary>
        /// [EDITOR ONLY] Save the creation of a link between two nodes
        /// </summary>
        /// <param name="_end"></param>
        /// <param name="_conversation"></param>
        public void EndTrace(Node _end, ConversationSO _conversation)
        {
            if (_end != null)
            {
                end = _end;
                isMoving = false;
                Save(_conversation);
            }
        }


        /// <summary>
        /// [EDITOR ONLY] Add a condition on an existing link
        /// </summary>
        public void AddCondition()
        {
            Condition cond = new Condition();
            conditions.Add(cond);
        }

        /// <summary>
        /// [EDITOR ONLY] Add an impact on an existing link
        /// </summary>
        public void AddImpact()
        {
            Impact imp = new Impact();
            impacts.Add(imp);
        }


        /// <summary>
        /// [EDITOR ONLY] Draw the link in the narrator window
        /// </summary>
        /// <param name="_mousePos"></param>
        public void Draw(Vector2 _mousePos)
        {
            Vector3 startPos;
            Vector3 endPos;

            startPos = new Vector3(start.contents[startBoxIndex].exitBox.x + start.contents[startBoxIndex].exitBox.width / 2, start.contents[startBoxIndex].exitBox.y + start.contents[startBoxIndex].exitBox.height / 2, 0);


            if (isMoving == true)
                endPos = new Vector3(_mousePos.x, _mousePos.y, 0);
            else
                endPos = new Vector3(end.entryBox.x + end.entryBox.width / 2, end.entryBox.y + end.entryBox.height / 2, 0);

            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

#if UNITY_EDITOR
            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 1);
#endif
            linkRect.x = (startPos.x + endPos.x) * 0.5f - linkRect.width * 0.5f;
            linkRect.y = (startPos.y + endPos.y) * 0.5f - linkRect.height * 0.5f;
        }

        /// <summary>
        /// [EDITOR ONLY] Save the link in it's conversation
        /// </summary>
        /// <param name="_conversation"></param>
        public void Save(ConversationSO _conversation)
        {
            _conversation.AddLinkToDialog(start, end, startBoxIndex);
        }   


        /// <summary>
        /// [EDITOR ONLY] Display the conditions & impacts window if selected
        /// </summary>
        public void IsSelected()
        {
            linkRect.width = 200.0f;
            linkRect.height = 200.0f;
        }

        /// <summary>
        /// [EDITOR ONLY] Hide the conditions & impacts window if unselected
        /// </summary>
        public void IsUnselected()
        {
            linkRect.width = 20.0f;
            linkRect.height = 20.0f;
        }
    }
}
