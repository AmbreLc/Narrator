using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Narrator
{
    public class Link
    {
        public Node start;
        public Node end;
        public Color color;
        public short startBoxIndex = 0;

        public Rect linkRect;

        public List<Condition> conditions;

        

        public bool moving;

        public Link()
        {
            start = null;
            end = null;
            color = Color.white;
            moving = true;

            linkRect = new Rect(0.0f, 0.0f, 20.0f, 20.0f);
            conditions = new List<Condition>();
        }

        public Link(Node _start, int _startIndex, Color _color)
        {
            start = _start;
            end = null;
            color = _color;
            moving = true;
            startBoxIndex = (short)_startIndex;

            linkRect = new Rect(0.0f, 0.0f, 20.0f, 20.0f);
            conditions = new List<Condition>();
        }

        public Link(Node _start, int _startIndex, Node _end, Color _color) 
        {
            start = _start;
            end = _end;
            color = _color;
            moving = false;
            startBoxIndex = (short)_startIndex;

            linkRect = new Rect(0.0f, 0.0f, 20.0f, 20.0f);
            conditions = new List<Condition>();
        }

        public void EndTrace(Node _end, ConversationSO _conversation)
        {
            if (_end != null)
            {
                end = _end;
                moving = false;
                Save(_conversation);
            }
        }


        public void AddCondition(Condition _condition)
        {

        }

        public void UpdateCondition(int _index, Condition _newCondition)
        {

        }

        public void Draw(Vector2 _mousePos)
        {
            Vector3 startPos;
            Vector3 endPos;

            startPos = new Vector3(start.contents[startBoxIndex].exitBox.x + start.contents[startBoxIndex].exitBox.width / 2, start.contents[startBoxIndex].exitBox.y + start.contents[startBoxIndex].exitBox.height / 2, 0);


            if (moving == true)
                endPos = new Vector3(_mousePos.x, _mousePos.y, 0);
            else
                endPos = new Vector3(end.entryBox.x + end.entryBox.width / 2, end.entryBox.y + end.entryBox.height / 2, 0);

            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 1);

            linkRect.x = (startPos.x + endPos.x) * 0.5f - linkRect.width * 0.5f;
            linkRect.y = (startPos.y + endPos.y) * 0.5f - linkRect.height * 0.5f;
        }

        public void Save(ConversationSO _conversation)
        {
            _conversation.AddLinkToDialog(start, end, startBoxIndex);
        }

        public void Delete()
        {

        }
    }
}
