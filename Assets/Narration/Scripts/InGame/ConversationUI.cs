using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Narrator
{

    public class ConversationUI : MonoBehaviour
    {
        [SerializeField] private ConversationSO conversation;
        private Node currentNode = new Node();

        [SerializeField] private bool displayInterlocutor = true;
        [SerializeField] private Text targetText;

        [SerializeField] private Text speakerText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button[] choicesButtons;

        List<string> choices = new List<string>();


        // Use this for initialization
        void Start()
        {
            currentNode = conversation.Entry;
            GoToNextNode();
        }

        // Update is called once per frame
        void Update()
        {
            if (currentNode.type == Node.Type.choice)
                DisplayChoices();
            else if(currentNode.type == Node.Type.speak)
                DisplayDialog();
        }

        [MenuItem("GameObject/Narrator/Conversation UI", false, 10)]
        static void AddUIToScene()
        {

        }


        void DisplayChoices()
        {
            targetText.enabled = false;
            nextButton.enabled = false;

            for (int i = 0; i < choices.Count; i++)
            {
                choicesButtons[i].gameObject.SetActive(true);
                choicesButtons[i].GetComponentInChildren<Text>().text = choices[i];
            }
        }

        void DisplayDialog()
        {
            if (speakerText.enabled = displayInterlocutor == true)
                speakerText.text = currentNode.charac.Name;

            targetText.enabled = true;
            nextButton.enabled = true;

            SpeakNode speakNode = currentNode as SpeakNode;
            targetText.text = choices[0];

            for (int i = 0; i < choicesButtons.Length; i++)
                choicesButtons[i].gameObject.SetActive(false);
        }

        public void GoToNextNode()
        {
            
            if (currentNode.contents[0].nextNodes.Count == 0)
            {
                Debug.Log("fin de la conversation");
                gameObject.SetActive(false);
            }
            else
            {
                int indexNextNode = 0;
                // déterminer le next node

                // Actualisation du noeud courant
                currentNode = conversation.Dialogs.dictionary[currentNode.contents[0].nextNodes[indexNextNode].index];
                // Actualisation du (des) texte(s) affiché(s)
                choices.Clear();
                for(int i = 0; i < currentNode.contents.Count; i++)
                    choices.Add(currentNode.contents[i].text);
            }
        }

        public void ChoseNextNode(int _index)
        {

            if (currentNode.contents[_index].nextNodes.Capacity == 0)
            {
                Debug.Log("fin de la conversation");
                gameObject.SetActive(false);
            }
            else
            {
                int indexNextNode = 0;
                // déterminer le next node

                // Actualisation du noeud courant
                currentNode = conversation.Dialogs.dictionary[currentNode.contents[_index].nextNodes[indexNextNode].index];
                // Actualisation du (des) texte(s) affiché(s)
                choices.Clear();
                for (int i = 0; i < currentNode.contents.Count; i++)
                    choices.Add(currentNode.contents[i].text);
            }
        }

    }

}
