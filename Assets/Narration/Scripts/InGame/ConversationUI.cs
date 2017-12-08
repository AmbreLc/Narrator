using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Narrator
{

    public class ConversationUI : MonoBehaviour
    {
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
        [SerializeField] private Text targetText;

        [SerializeField] private Text speakerText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button[] choicesButtons;

        //[Header("Informations")]
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
            targetText.enabled = false;
            nextButton.gameObject.SetActive(false);

            for (int i = 0; i < choicesButtons.Length; i++)
            {
                if (i < choices.Count)
                {
                    choicesButtons[i].gameObject.SetActive(true);
                    choicesButtons[i].GetComponentInChildren<Text>().text = choices[i];
                }
                else
                    choicesButtons[i].gameObject.SetActive(false);
            }

        }

        void DisplayDialog()
        {
            if (speakerText.enabled = displayInterlocutor == true)
                speakerText.text = currentNode.charac.Name;

            targetText.enabled = true;
            nextButton.gameObject.SetActive(true);

            SpeakNode speakNode = currentNode as SpeakNode;
            targetText.text = choices[0];

            for (int i = 0; i < choicesButtons.Length; i++)
                choicesButtons[i].gameObject.SetActive(false);

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
                choices.Add(currentNode.contents[i].texts[brain.CurrentLangageIndex]);
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
