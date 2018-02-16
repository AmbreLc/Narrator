/* NARRATOR PACKAGE : ConversationSO.cs
 * Created by Ambre Lacour
 * 
 * Scriptable object : contains all datas of a conversation
 * 
 * A conversation:
 *      - has a name
 *      - contains a list of SpeakNode
 *      - belongs in a ConversationGroup
 *      
 * You can create a conversation asset in your hierarchy and fill it with the narrator window
 */


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;


namespace Narrator
{
    [System.Serializable]
    public class ConversationSO : ScriptableObject
    {
        [SerializeField, HideInInspector] private string conversationName = "conversation";
        /// <summary>
        /// The conversation name
        /// </summary>
        public string ConversationName
        {
            get { return conversationName; }
            set { conversationName = value; }
        }

        [SerializeField, HideInInspector] private EntryNode entry;
        /// <summary>
        /// The entry node of the conversation (no dialog, links to next nodes)
        /// </summary>
        public EntryNode Entry
        {
            get { return entry; }
        }

        [SerializeField, HideInInspector] private List<Node> dialogs;
        /// <summary>
        /// Conversations nodes (each node includes one content or more) 
        /// </summary>
        public List<Node> Dialogs
        {
            get { return dialogs; }
        }


        /// <summary>
        /// Get the number of dialog nodes in the conversation
        /// </summary>
        public int GetDialogsCount { get { return Dialogs.Count; } }

        /// <summary>
        /// [EDITOR ONLY] Add a dialog node to the conversation
        /// </summary>
        /// <param name="_newNode"></param>
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
            int startNodeIndex = -2;
            int endNodeIndex = -2;

            if (_start.type == Node.Type.entry)
            {
                startNodeIndex = -1;
            }

                for (int i = 0; i < dialogs.Count; i++)
            {
                if (startNodeIndex == -2 && dialogs[i] == _start)
                    startNodeIndex = i;
                else if (endNodeIndex == -2 && dialogs[i] == _end)
                    endNodeIndex = i + 1;
            }

            if (startNodeIndex == -2 || endNodeIndex == -2)
                Debug.LogError("Error : dialog missing in conversation, cannot link");
            else if (startNodeIndex == -1)
                entry.contents[_startIndex].AddNextNode(endNodeIndex);
            else
                dialogs[startNodeIndex].contents[_startIndex].AddNextNode(endNodeIndex);
        }


        /// <summary>
        ///  [EDITOR ONLY] Delete a link between two dialog nodes
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_contentIndex"></param>
        public void DeleteLinkFromDialog(Node _start, Node _end, int _contentIndex)
        {
            int startIndex = -2;
            int endIndex = -2;

            // If the starting node is the entry one
            if (_start.type == Node.Type.entry)
                startIndex = -1;

            // if the start and/or end node(s) is/are speaknode(s)
            for (int i = 0; i < dialogs.Count; i++)
            {
                if (startIndex == -2 && _start == dialogs[i])
                    startIndex = i;             
                else if (endIndex == -2 && _end == dialogs[i])
                    endIndex = i + 1;
            }

            if (startIndex == -2 || endIndex == -2)
                Debug.LogError("Error : dialog missing in conversation, cannot link");
            else if (startIndex == -1)
                entry.contents[_contentIndex].RemoveNextNode(endIndex);
            else
                dialogs[startIndex].contents[_contentIndex].RemoveNextNode(endIndex);

            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Delete a node in the conversation and all its links
        /// </summary>
        /// <param name="_node"></param>
        public void DeleteNodeFromDialog(Node _node)
        {
            if (Dialogs.Contains(_node))
            {
                int nodeIndex = 0;
                for (int i = 0; i < Dialogs.Count; i++)
                {
                    if (Dialogs[i] == _node)
                        nodeIndex = i + 1;
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


        /// <summary>
        /// [EDITOR ONLY] Add a condition on a link between two nodes
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_contentIndex"></param>
        /// <param name="_nextNodeIndex"></param>
        /// <param name="_condition"></param>
        public void AddCondition(Node _start, int _contentIndex, int _nextNodeIndex, Condition _condition)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].conditions.Add(_condition);
            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Update a condition on a link between two nodes
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_contentIndex"></param>
        /// <param name="_nextNodeIndex"></param>
        /// <param name="_conditionIndex"></param>
        /// <param name="_condition"></param>
        public void UpdateCondition(Node _start, int _contentIndex, int _nextNodeIndex, int _conditionIndex, Condition _condition)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].conditions[_conditionIndex] = _condition;
            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Add an impact on a link between two nodes
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_contentIndex"></param>
        /// <param name="_nextNodeIndex"></param>
        /// <param name="_impact"></param>
        public void AddImpact(Node _start, int _contentIndex, int _nextNodeIndex, Impact _impact)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].impacts.Add(_impact);
            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Update an impact on a link between two nodes
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_contentIndex"></param>
        /// <param name="_nextNodeIndex"></param>
        /// <param name="_impactIndex"></param>
        /// <param name="_impact"></param>
        public void UpdateImpact(Node _start, int _contentIndex, int _nextNodeIndex, int _impactIndex, Impact _impact)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].impacts[_impactIndex] = _impact;
            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Create a new ConversationSO in the Asset folder
        /// </summary>
        public void CreateConversation()
        {
            conversationName = "New Conversation";
            entry = new EntryNode();
            entry.CreateEntryNode();
            dialogs = new List<Node>();

            Save();
        }

        /// <summary>
        /// [EDITOR ONLY] Save all conversations modifications
        /// </summary>
        private void Save()
        {
#if UNITY_EDITOR
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
                    return Dialogs[_currentNode.contents[_contentIndex].nextNodes[i].index - 1];
                }
            }

            return null;
        }
  
    }

}
