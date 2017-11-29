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

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace Narrator
{
    [CreateAssetMenu(fileName = "Conversation.asset", menuName = "Conversation")]
    [System.Serializable]
    public class ConversationSO : ScriptableObject
    {
        [SerializeField] private string conversationName;
        public string ConversationName
        {
            get { return conversationName; }
            set { conversationName = value; EditorUtility.SetDirty(this);
            }
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

        [SerializeField] private List<Character> npcs;
        public List<Character> NPCs
        {
            get { return npcs; }
        }

        [SerializeField] private List<Character> pcs;
        public List<Character> PCs
        {
            get { return pcs; }
        }


        [SerializeField] private Parameters parameters;
        public Parameters Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }


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

        public void DeleteLinkFromDialog(Node _start, Node _end, int _startIndex)
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
                entry.contents[_startIndex].RemoveNextNode(endIndex);
            else
                dialogs.dictionary[startIndex].contents[_startIndex].RemoveNextNode(endIndex);
        
        }

        public void AddCharacter(Character _character)
        {
            if (_character.IsPlayable == true && PCs.Contains(_character) == false)
                PCs.Add(_character);
            else if (_character.IsPlayable == false && NPCs.Contains(_character) == false)
                NPCs.Add(_character);
        }
        public void DeleteCharacter(Character _character)
        {
            if (_character.IsPlayable == true && PCs.Contains(_character) == true)
                PCs.Remove(_character);
            else if (_character.IsPlayable == false && NPCs.Contains(_character) == true)
                NPCs.Remove(_character);

        }


        public void CreateConversation()
        {
            conversationName = "New Conversation";
            dialogsCount = 0;
            entry = new EntryNode();
            entry.CreateEntryNode();
            dialogs = Dialogs.New<Dialogs>();

            npcs = new List<Character>();
            pcs = new List<Character>();
            Color play1 = new Color(0.8f, 0.8f, 1.0f);
            pcs.Add(new Character("Player1", true, play1));

            parameters = new Parameters();

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        

    }

}
