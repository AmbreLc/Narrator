using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Narrator
{
    [System.Serializable]
    public class ConversationUI : MonoBehaviour
    {
        [System.Serializable]
        struct CharacterUI
        {
            public Text speakerText;
            public Text contentText;
            public Button nextButton;
            public Button[] choicesButtons;
        }

        enum DisplayType
        {
            blunt,
            fade
        }

        enum State
        {
            fadeIn,
            current,
            fadeOut
        }
        State state = State.fadeIn;

        [Header("Scriptable objects")]
        [SerializeField] private NarratorBrainSO brain;
        [SerializeField] private ConversationSO conversation;


        [Header("UI elements")]
        [SerializeField] private DisplayType displayType;

        [SerializeField] private bool displayInterlocutor = true;
        [SerializeField] private CharacterUI charactersUI;

        private Node currentNode = new Node();
        List<string> choices = new List<string>();

        private bool isOver = false;
        public bool IsOver { get { return isOver; } }

        private float fading;



        void Start()
        {
            if (conversation == null)
            {
                this.enabled = false;
                Debug.LogError("ConversationUI : missing ConversationSO, can't display dialogs. Component has been disabled.");
            }
            else if (brain == null)
            {
                this.enabled = false;
                Debug.LogError("ConversationUI : missing BrainSO, can't display dialogs. Component has been disabled.");
            }
            else
            {
                Init();
            }
        }

        public void Init()
        {
            Debug.Assert(charactersUI.choicesButtons != null, "No button to display choices");
            Debug.Assert(charactersUI.contentText != null, "No UI to display dialog text");
            Debug.Assert(charactersUI.nextButton != null, "No UI to display next button");
            Debug.Assert(charactersUI.speakerText != null, "No UI to display speaker text");

            isOver = false;
            currentNode = conversation.Entry;
            GoToNextNode(0);

            if (displayType == DisplayType.fade)
            {
                state = State.fadeIn;
                fading = 0.0f;
            }
            else
            {
                state = State.current;
                fading = 1.0f;
            }
        }

        public void NewConversation(ConversationSO _conv)
        {
            conversation = _conv;
            Init();
        }


        void Update()
        {
            if (currentNode.type == Node.Type.choice)
                DisplayChoices();
            else if (currentNode.type == Node.Type.speak)
                DisplayDialog();


            if (displayType == DisplayType.fade)
            {
                SetOpacity();
                if (state == State.fadeIn)
                {
                    if (fading < 1.0f)
                        fading += Time.deltaTime;
                    else
                        state = State.current;
                }
                else if (state == State.fadeOut)
                {
                    if (fading > 0.0f)
                        fading -= Time.deltaTime;
                    else
                        EndConversation();
                }
            }

        }


        void DisplayChoices()
        {

            charactersUI.contentText.enabled = false;
            charactersUI.nextButton.gameObject.SetActive(false);

            for (int i = 0; i < charactersUI.choicesButtons.Length; i++)
            {
                if (i < choices.Count)
                {
                    charactersUI.choicesButtons[i].gameObject.SetActive(true);
                    charactersUI.choicesButtons[i].GetComponentInChildren<Text>().text = choices[i];
                }
                else
                    charactersUI.choicesButtons[i].gameObject.SetActive(false);
            }

        }

        void DisplayDialog()
        {

                if (charactersUI.speakerText.enabled = displayInterlocutor == true)
                    charactersUI.speakerText.text = currentNode.charac.Name;

                charactersUI.contentText.text = choices[0];
                charactersUI.contentText.enabled = true;

                charactersUI.nextButton.gameObject.SetActive(true);

                for (int i = 0; i < charactersUI.choicesButtons.Length; i++)
                    charactersUI.choicesButtons[i].gameObject.SetActive(false);

        }

        public void GoToNextNode(int _index)
        {
            // Update current node
            currentNode = conversation.GoToNextNode(brain, currentNode, _index);

            // Test : end of the conversation
            if(currentNode == null)
            {
                switch(displayType)
                {
                    case DisplayType.blunt:
                        EndConversation();
                        break;
                    case DisplayType.fade:
                        state = State.fadeOut;
                        break;
                }
            }
            else
                UpdateChoicesList();
                
        }

        void EndConversation()
        {
            isOver = true;
            fading = 0.0f;
            SetOpacity();
            currentNode = conversation.Entry;
        }

        void UpdateChoicesList()
        {
            choices.Clear();
            for (int i = 0; i < currentNode.contents.Count; i++)
            {
                choices.Add(currentNode.contents[i].texts[brain.CurrentLanguageIndex]);
            }
        }

        void SetOpacity()
        {
            foreach (Image i in GetComponentsInChildren<Image>())
                i.color = new Color(i.color.r, i.color.g, i.color.b, fading);
            foreach (Text t in GetComponentsInChildren<Text>())
                t.color = new Color(t.color.r, t.color.g, t.color.b, fading);
        }
    }

}
