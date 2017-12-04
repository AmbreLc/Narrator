﻿/* NARRATOR PACKAGE
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
    [CreateAssetMenu(fileName = "Conversation.asset", menuName = "Narrator/Conversation")]
#endif

    [System.Serializable]
    public class ConversationSO : ScriptableObject
    {
        [SerializeField] private string conversationName;
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

        [SerializeField] private Dialogs dialogs;
        public Dialogs Dialogs
        {
            get { return dialogs; }
        }

        [SerializeField] private int dialogsCount;


        public void AddDialogNode(Node _newNode)
        {
            dialogsCount = dialogs.dictionary.Count + 1;
            dialogs.dictionary.Add(dialogsCount, _newNode);
            Debug.Log("Add dialog, index " + dialogsCount);
        }

        /// <summary>
        /// Add a next node index to an existent node
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        public void AddLinkToDialog(Node _start, Node _end, int _startIndex)
        {
            int startIndex = -1;
            int endIndex = -1;

            // If the starting node is the entry one
            if (_start.type == Node.Type.entry)
                startIndex = 0;

            // if the start and/or end node(s) is/are speaknode(s)
            foreach (KeyValuePair<int, Node> entry in dialogs.dictionary)
            {
                if(entry.Value.windowRect == _start.windowRect)
                {
                    startIndex = entry.Key;
                }
                else if(entry.Value.windowRect == _end.windowRect)
                {
                    endIndex = entry.Key;
                }
            }

            if (startIndex == -1 || endIndex == -1)
                Debug.LogError("Error : dialog missing in conversation, cannot link");
            else if (startIndex == 0)
                entry.contents[_startIndex].AddNextNode(endIndex);
            else
            {
                dialogs.dictionary[startIndex].contents[_startIndex].AddNextNode(endIndex);
            }
        }

        public void DeleteLinkFromDialog(Node _start, Node _end, int _contentIndex)
        {
            int startIndex = -1;
            int endIndex = -1;

            // If the starting node is the entry one
            if (_start.type == Node.Type.entry)
                startIndex = 0;

            // if the start and/or end node(s) is/are speaknode(s)
            foreach (KeyValuePair<int, Node> entry in dialogs.dictionary)
            {
                if (entry.Value.windowRect == _start.windowRect)
                {
                    startIndex = entry.Key;
                }
                else if (entry.Value.windowRect == _end.windowRect)
                {
                    endIndex = entry.Key;
                }
            }

            if (startIndex == -1 || endIndex == -1)
                Debug.LogError("Error : dialog missing in conversation, cannot link");
            else if (startIndex == 0)
                entry.contents[_contentIndex].RemoveNextNode(endIndex);
            else
                dialogs.dictionary[startIndex].contents[_contentIndex].RemoveNextNode(endIndex);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

        }


        public void AddCondition(Node _start, int _contentIndex, int _nextNodeIndex, Condition _condition)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].conditions.Add(_condition);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public void UpdateCondition(Node _start, int _contentIndex, int _nextNodeIndex, int _conditionIndex, Condition _condition)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].conditions[_conditionIndex] = _condition;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }


        public void AddImpact(Node _start, int _contentIndex, int _nextNodeIndex, Impact _impact)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].impacts.Add(_impact);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public void UpdateImpact(Node _start, int _contentIndex, int _nextNodeIndex, int _impactIndex, Impact _impact)
        {
            _start.contents[_contentIndex].nextNodes[_nextNodeIndex].impacts[_impactIndex] = _impact;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }


        public void CreateConversation()
        {
            conversationName = "New Conversation";
            dialogsCount = 0;
            entry = new EntryNode();
            entry.CreateEntryNode();
            dialogs = Dialogs.New<Dialogs>();


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
                    return Dialogs.dictionary[_currentNode.contents[_contentIndex].nextNodes[i].index];
                }
            }

            return null;
        }
  
    }

}