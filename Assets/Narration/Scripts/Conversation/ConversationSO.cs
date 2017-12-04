/* NARRATOR PACKAGE
 * ConversationSO.cs
 * Created by Ambre Lacour, 12/10/2017
 * Scriptable object : contains all datas of a conversation
 * 
 * A conversation:
 *      - has a name
 *      - contains a list of SpeakNode
 *      - belongs in a ConversationGroup
 *      
 * You can create a conversation asset in your hierarchy and fill it with the narrator
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
#if UNITY_EDITOR
    // bugged because of SerializedDictionary not initialized when asset generated
    //[CreateAssetMenu(fileName = "Conversation.asset", menuName = "Narrator/Conversation")]
#endif

    [System.Serializable]
    public class ConversationSO : ScriptableObject
    {
        [SerializeField] private string conversationName = "conversation";
        public string ConversationName
        {
            get { return conversationName; }
            set { conversationName = value; /*EditorUtility.SetDirty(this);*/ }
        }

        [SerializeField] private EntryNode entry;
        public EntryNode Entry
        {
            get { return entry; }
        }

        [SerializeField] private List<Node> dialogs;
        public List<Node> Dialogs
        {
            get { return dialogs; }
        }

        public int GetDialogsCount { get { return Dialogs.Count; } }


        public void AddDialogNode(Node _newNode)
        {
            dialogs.Add(_newNode);
        }

        /// <summary>
        /// Add a next node index to an existent node
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        public void AddLinkToDialog(Node _start, Node _end, int _startIndex)
        {
            // Obsolete
            {
                /*
                int startIndex = -1;
                int endIndex = -1;

                // If the starting node is the entry one
                if (_start.type == Node.Type.entry)
                    startIndex = 0;

                // if the start and/or end node(s) is/are speaknode(s)
                foreach (Node node in dialogs)
                {
                    if (node.windowRect == _start.windowRect)
                    {
                        startIndex = node.ID;
                    }
                    else if (node.windowRect == _end.windowRect)
                    {
                        endIndex = node.ID;
                    }
                }

                if (startIndex == -1 || endIndex == -1)
                    Debug.LogError("Error : dialog missing in conversation, cannot link");
                else if (startIndex == 0)
                    entry.contents[_startIndex].AddNextNode(endIndex);
                else
                {
                    dialogs[startIndex].contents[_startIndex].AddNextNode(endIndex);
                }
                */
            }

            int startNodeIndex = -1;
            int endNodeIndex = -1;

            if (_start.type == Node.Type.entry)
                startNodeIndex = 0;

            for (int i = 0; i < dialogs.Count; i++)
            {
                if (startNodeIndex == -1 && dialogs[i] == _start)
                    startNodeIndex = i;
                else if (endNodeIndex == -1 && dialogs[i] == _end)
                    endNodeIndex = i + 1;
            }

            if (startNodeIndex == -1 || endNodeIndex == -1)
                Debug.LogError("Error : dialog missing in conversation, cannot link");
            else if (startNodeIndex == 0)
                entry.contents[_startIndex].AddNextNode(endNodeIndex);
            else
                dialogs[startNodeIndex].contents[_startIndex].AddNextNode(endNodeIndex);
        }


        public void DeleteLinkFromDialog(Node _start, Node _end, int _contentIndex)
        {
            int startIndex = -1;
            int endIndex = -1;

            // If the starting node is the entry one
            if (_start.type == Node.Type.entry)
                startIndex = 0;

            // if the start and/or end node(s) is/are speaknode(s)
            for (int i = 0; i < dialogs.Count; i++)
            {
                if (startIndex == -1 && _start == dialogs[i])
                    startIndex = i;             
                else if (endIndex == -1 && _end == dialogs[i])
                    endIndex = i;
            }

            if (startIndex == -1 || endIndex == -1)
                Debug.LogError("Error : dialog missing in conversation, cannot link");
            else if (startIndex == 0)
                entry.contents[_contentIndex].RemoveNextNode(endIndex);
            else
                dialogs[startIndex].contents[_contentIndex].RemoveNextNode(endIndex);

            Save();
        }

        public void DeleteNodeFromDialog(Node _node)
        {
            if (Dialogs.Contains(_node))
            {
                int nodeIndex = 0;
                for (int i = 0; i < Dialogs.Count; i++)
                {
                    if (Dialogs[i] == _node)
                        nodeIndex = i;
                }
               
                bool goBreak = true;
                while (goBreak == true)
                {
                    goBreak = false;
                    for (int i = 0; i < Entry.contents.Count; i++)
                    {
                        if (goBreak == true) break;
                        for (int j = 0; j < Entry.contents[i].nextNodes.Count; j++)
                        {
                            if (goBreak == true) break;
                            if (Entry.contents[i].nextNodes[j].index == nodeIndex)
                            {
                                Entry.contents[i].nextNodes.RemoveAt(j);
                                goBreak = true;
                            }
                        }
                    }
                }

                goBreak = true;
                while (goBreak == true)
                {
                    goBreak = false;
                    for (int a = 0; a < Dialogs.Count; a++)
                    {
                        if (goBreak == true) break;
                        for (int i = 0; i < Dialogs[a].contents.Count; i++)
                        {
                            if (goBreak == true) break;
                            for (int j = 0; j < Dialogs[a].contents[i].nextNodes.Count; j++)
                            {
                                if (goBreak == true) break;
                                if (Dialogs[a].contents[i].nextNodes[j].index == nodeIndex)
                                {
                                    Dialogs[a].contents[i].nextNodes.RemoveAt(j);
                                    goBreak = true;
                                }
                            }
                        }
                    }
                }

            }
            else
                Debug.LogError("Conversation doesn't contain the node you're trying to delete");
                  
            
            Dialogs.Remove(_node);
            Save();
        }



        public void AddCondition(Node _start, int _contentIndex, int _nextNodeIndex, Condition _condition)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].conditions.Add(_condition);
            Save();
        }

        public void UpdateCondition(Node _start, int _contentIndex, int _nextNodeIndex, int _conditionIndex, Condition _condition)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].conditions[_conditionIndex] = _condition;
            Save();
        }


        public void AddImpact(Node _start, int _contentIndex, int _nextNodeIndex, Impact _impact)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].impacts.Add(_impact);
            Save();
        }

        public void UpdateImpact(Node _start, int _contentIndex, int _nextNodeIndex, int _impactIndex, Impact _impact)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].impacts[_impactIndex] = _impact;
            Save();
        }


        public void CreateConversation()
        {
            conversationName = "New Conversation";
            entry = new EntryNode();
            entry.CreateEntryNode();
            dialogs = new List<Node>();

            Save();
        }

        private void Save()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        /// <summary>
        /// Return the next node of the conversation, or null if there is none
        /// </summary>
        /// <param name="_brain"></param>
        /// <param name="_currentNode"></param>
        /// <param name="_contentIndex"></param>
        /// <param name="_applyImpact"></param>
        /// <returns></returns>
        public Node GoToNextNode(NarratorBrainSO _brain, Node _currentNode, int _contentIndex, bool _applyImpact = true)
        {
            for (int i = 0; i < _currentNode.contents[_contentIndex].nextNodes.Count; i++)
            {
                bool canGoNextNode = true;

                for (int j = 0; j < _currentNode.contents[_contentIndex].nextNodes[i].conditions.Count; j++)
                {
                    if (_currentNode.contents[_contentIndex].nextNodes[i].conditions[j].IsComplete(_brain.Parameters) == false)
                    {
                        canGoNextNode = false;
                    }
                }
                if (canGoNextNode)
                {
                    for (int j = 0; j < _currentNode.contents[_contentIndex].nextNodes[i].impacts.Count; j++)
                    {
                        _brain.ApplyImpact(_currentNode.contents[_contentIndex].nextNodes[i].impacts[j]);
                    }
                    return Dialogs[_currentNode.contents[_contentIndex].nextNodes[i].index];
                }
            }

            return null;
        }
  
    }

}
