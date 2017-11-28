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


        // Use this for initialization
        void Start()
        {
            currentNode = conversation.Entry;
            GetNextNode();
        }

        // Update is called once per frame
        void Update()
        {
            if (currentNode.charac.IsPlayable == true)
                DisplayChoices();
            else
                DisplayDialog();
        }

        [MenuItem("GameObject/Narrator/Conversation UI", false, 10)]
        static void AddUIToScene()
        {

        }


        void DisplayChoices()
        {
            targetText.enabled = false;

            ChoiceNode choiceNode = currentNode as ChoiceNode;
            for (int i = 0; i < choiceNode.exitBoxes.Count; i++)
            {
                choicesButtons[i].gameObject.SetActive(true);
                choicesButtons[i].GetComponentInChildren<Text>().text = choiceNode.choices[i];
            }



        }

        void DisplayDialog()
        {
            if (speakerText.enabled = displayInterlocutor == true)
                speakerText.text = currentNode.charac.Name;

            targetText.enabled = true;
            SpeakNode speakNode = currentNode as SpeakNode;
            targetText.text = speakNode.speak;

            for (int i = 0; i < choicesButtons.Length; i++)
                choicesButtons[i].gameObject.SetActive(false);
        }

        public void GetNextNode()
        {
            
            if (currentNode.exitBoxes[0].nextNodes.Capacity == 0)
            {
                Debug.Log("fin de la conversation");
                gameObject.SetActive(false);
            }
            else
            {
                int indexNextNode = 0;
                // déterminer le next node
                currentNode = conversation.Dialogs.dictionary[currentNode.exitBoxes[0].nextNodes[indexNextNode]];
            }
        }

    }

}
