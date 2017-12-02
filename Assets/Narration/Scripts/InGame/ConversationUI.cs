using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Narrator
{

    public class ConversationUI : MonoBehaviour
    {
        [SerializeField] private NarratorBrainSO brain;
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

            // Choix du next node
            int nextNodeIndex = TestNextNodes(0);

            if (nextNodeIndex == -1)
            {
                Debug.Log("fin de la conversation");
                gameObject.SetActive(false);
            }
            else
            {
                // Actualisation du noeud courant
                currentNode = conversation.Dialogs.dictionary[currentNode.contents[0].nextNodes[nextNodeIndex].index];
                // Actualisation du (des) texte(s) affiché(s)
                choices.Clear();
                for(int i = 0; i < currentNode.contents.Count; i++)
                    choices.Add(currentNode.contents[i].text);
            }
        }

        public void ChoseNextNode(int _index)
        {        
            // Choix du next node
            int nextNodeIndex = TestNextNodes(_index);

            if (nextNodeIndex == -1)
            {
                Debug.Log("fin de la conversation");
                gameObject.SetActive(false);
            }
            else
            {
                // Actualisation du noeud courant
                currentNode = conversation.Dialogs.dictionary[currentNode.contents[_index].nextNodes[nextNodeIndex].index];
                // Actualisation du (des) texte(s) affiché(s)
                choices.Clear();
                for (int i = 0; i < currentNode.contents.Count; i++)
                    choices.Add(currentNode.contents[i].text);
            }
        }


        int TestNextNodes(int _contentIndex)
        {
            for (int i = 0; i < currentNode.contents[_contentIndex].nextNodes.Count; i++)
            {
                bool canGoNextNode = true;

                for (int j = 0; j < currentNode.contents[_contentIndex].nextNodes[i].conditions.Count; j++)
                {
                    if (currentNode.contents[_contentIndex].nextNodes[i].conditions[j].IsComplete(brain.Parameters) == false)
                    {
                        canGoNextNode = false;
                    }
                }
                if (canGoNextNode)
                {
                    for (int j = 0; j < currentNode.contents[_contentIndex].nextNodes[i].impacts.Count; j++)
                    {
                        brain.ApplyImpact(currentNode.contents[_contentIndex].nextNodes[i].impacts[j]);
                    }
                    return i;
                }
            }

            return -1;
        }
    }

}
