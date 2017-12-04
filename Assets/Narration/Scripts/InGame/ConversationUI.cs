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




        // Use this for initialization
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
            }
        }

        public void NewConversation(ConversationSO _conv)
        {
            conversation = _conv;
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (currentNode.type == Node.Type.choice)
                DisplayChoices();
            else if (currentNode.type == Node.Type.speak)
                DisplayDialog();


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
                state = State.fadeOut;
            }

            // Obsolete
            {
                /*
                int nextNodeIndex = TestNextNodes(_index);
                if (nextNodeIndex == -1)
                {
                    state = State.fadeOut;
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
                */
            }
        }

        void EndConversation()
        {
            isOver = true;
            currentNode = conversation.Entry;
        }

        // Obsolete : moved to ConversationSO
        
        /*
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
        */
        


        void SetOpacity()
        {
            foreach (Image i in GetComponentsInChildren<Image>())
                i.color = new Color(i.color.r, i.color.g, i.color.b, fading);
            foreach (Text t in GetComponentsInChildren<Text>())
                t.color = new Color(t.color.r, t.color.g, t.color.b, fading);
        }
    }

}
