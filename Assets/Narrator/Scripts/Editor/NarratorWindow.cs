/* NARRATOR PACKAGE
 * NarratorWindow.cs
 * Created by Ambre Lacour, 05/10/2017
 * Editor window : Display & edit conversations
 * 
 * The Narrator window :
 *      - loads all the ConversationSO it can find in Assets folder
 *      
 *      - loads or generates a Conversation Brain
 *      
 *      - can creates conversations
 *      
 *      - can edits conversations i.e. :
 *             =>create/edit/remove dialogs,
 *             =>create/edit/remove links between dialogs

 *      - can edit brain i.e. :
 *             =>add/edit/remove characters
 *             =>add/edit/remove parameters
 */


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Narrator
{
    public class NarratorWindow : EditorWindow
    {

        // windowID: distinguish the different windows
        enum windowID
        {
            conversations,
            characters,
            transitions,
            entryNode,
            dialogs,
        }

        // Mouse & inputs infos

        private Vector2 mousePos;
        private float zoomValue = 1.0f;
        private Vector2 zoomPos = Vector2.one;
        private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        private Vector2 scrollStartMousePos;


        // Init info
        bool hasBeenInit = false;


        // Left window

        // Conversations list
        Rect leftWindow_Up = new Rect(0.0f, 0.0f, 200.0f, 200.0f);
        List<ConversationSO> conversationList;
        string[] conversationsNames;
        int currentConversationIndex = 0;
        Vector2 convListScrollVect = Vector2.zero;        // conversations' scrollbar value

        // Langages
        int convOrLangages = 0;
        string[] convOrLang = new string [2];

        // Conversation brain : Characters & params list
        NarratorBrainSO brain;
        Rect leftWindow_Down = new Rect(0.0f, 200.0f, 200.0f, 250.0f);
        Rect charOrParamsRect = new Rect(0.0f, 0.0f, 200.0f, 20.0f);
        int charOrParamsIndex = 0;
        string[] charOrParams = new string[2];
        List<Character> characters = new List<Character>();
        Vector2 playersListScrollVec = Vector2.zero;        // players' scrollbar value
        Vector2 npcsListScrollVec = Vector2.zero;           // NPCs' scrollbar value
        Vector2[] paramsListScrollVec = new Vector2[3];     // params' scrollbars values


        // Main window

        // Background
        private Rect backgroundRect = new Rect(0.0f, 0.0f, 800.0f, 800.0f);
        private Vector2 backOffset;
        private Vector2 backDrag;

        // Dialogs
        private ConversationSO currentConv;
        private int selectedNodeIndex = -1;

        // Links
        private List<Link> links = new List<Link>();
        private Link tempLink;
        private Color linkColor = Color.white;

        // Selected link window
        private int selectedLinkIndex = -1;
        private int currentCondition = 0;


        // Menu item : display the Narrator window from the menu button
        [MenuItem("Window/Narrator")]
        static void ShowEditor()
        {
            NarratorWindow editor = GetWindow<NarratorWindow>("Narrator");
            editor.stopWatch.Start();
            editor.Start();
        }

        /// <summary>
        /// Called once to initialize editor window 
        /// </summary>
        private void Start()
        {
            zoomPos = Vector2.zero;

            LoadConversationBrain();

            InitializeLeftWindow_Conversations();
            InitializeLeftWindow_CharAndParams();
            SelectedLink_Initialize();

            currentConv = CreateInstance<ConversationSO>();
            currentConv.CreateConversation();
            UpdateCurrentConversation();
            NarratorPopUp.popupDel += RenameLanguageEnd;

            hasBeenInit = true;
        }


        private void Update()
        {
            if (hasBeenInit == false)
                Start();
            stopWatch.Reset();
            stopWatch.Start();
            Repaint();
        }


        void OnGUI()
        {
            if (conversationList == null) // if Narrator window was already open when Unity was launched, it needs initialization
                Start();

            Event e = Event.current;
            mousePos = e.mousePosition;

            InputsHandler(e);

            BeginWindows();

            // Display background and left windows
            DrawWindowBackground();
            leftWindow_Up = GUI.Window((int)windowID.conversations, leftWindow_Up, DrawLeftWindow_ConvAndLangages, "");
            leftWindow_Down = GUI.Window((int)windowID.characters, leftWindow_Down, DrawLeftWindow_CharAndParams, "");



            // Display all links
            if (tempLink != null)
                tempLink.Draw(mousePos);
            for (int i = 0; i < links.Count; i++)
            {
                links[i].Draw(mousePos);
                GUI.Box(links[i].linkRect, "");
            }

            // Display all nodes
            if (conversationList.Count != 0)
            {
                if (currentConv.Entry != null)
                {
                    currentConv.Entry.windowRect = GUI.Window((int)windowID.entryNode, currentConv.Entry.windowRect, DrawEntryNode, "Entry");
                    currentConv.Entry.DrawBox();
                }
                for (int i = 0; i < currentConv.Dialogs.Count; i++)
                {
                    if (currentConv.Dialogs[i] != null)
                    {
                        GUI.backgroundColor = currentConv.Dialogs[i].charac.Color;
                        currentConv.Dialogs[i].windowRect = GUI.Window(i + (int)windowID.dialogs, currentConv.Dialogs[i].windowRect, DrawSpeakNode, currentConv.Dialogs[i].charac.Name);
                        currentConv.Dialogs[i].DrawBox();
                        GUI.backgroundColor = Color.white;
                    }
                }
            }

            // Display selected link infos
            if (selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].linkRect = GUI.Window((int)windowID.transitions, links[selectedLinkIndex].linkRect, DrawSelectedLink, "Transition");
            }

            EndWindows();

        }




        //________________ INPUTS & INTERACTIONS ________________//

        /// <summary>
        /// Main inputs fonction (check all inputs)
        /// </summary>
        /// <param name="e"></param>
        private void InputsHandler(Event e)
        {
            // Mouse clicks
            if (e.button == 1 && e.type == EventType.MouseDown)
                HandleRightClick(e);
            else if (e.button == 0 && e.type == EventType.MouseDown)
                HandleLeftClick();

            // Mouse drag & scroll Wheel
            if (e.button == 2 & e.type == EventType.MouseDrag)
            {
                zoomPos = e.delta;
                OnDrag(zoomPos);
            }
            if (e.type == EventType.scrollWheel)
                zoomValue += e.delta.y * 0.1f;
        }

        /// <summary>
        /// Right click events
        /// </summary>
        /// <param name="e"></param>
        private void HandleRightClick(Event e)
        {
            // click on up left window
            if (leftWindow_Up.Contains(mousePos))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add conversation"), false, CreateConversation, "createConv");
                menu.ShowAsContext();
                e.Use();
            }

            // OR click on down left window (characters or parameters)
            else if (leftWindow_Down.Contains(mousePos))
            {

                GenericMenu menu = new GenericMenu();
                if (charOrParamsIndex == 0)
                {
                    menu.AddItem(new GUIContent("Add playable character"), false, CreateCharacter, true);
                    menu.AddItem(new GUIContent("Add non-playable character"), false, CreateCharacter, false);
                }
                else
                {
                    menu.AddItem(new GUIContent("Add float"), false, CreateParameter, Parameters.TYPE.f);
                    menu.AddItem(new GUIContent("Add int"), false, CreateParameter, Parameters.TYPE.i);
                    menu.AddItem(new GUIContent("Add bool"), false, CreateParameter, Parameters.TYPE.b);


                }
                menu.ShowAsContext();
                e.Use();

            }

            // OR click on the main window
            else
            {
                // Click on a link already selected
                bool clickedOnTransition = false;
                if (selectedLinkIndex != -1)
                {
                    if (links[selectedLinkIndex].linkRect.Contains(mousePos))
                    {
                        clickedOnTransition = true;

                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add condition"), false, AddConditionOnLink);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Add impact"), false, AddImpactOnLink);
                        menu.ShowAsContext();
                        e.Use();
                    }
                }

                // OR click on a dialog node
                bool clickedOnWindow = false;
                if (clickedOnTransition == false)
                {
                    for (int i = 0; i < currentConv.Dialogs.Count; i++)
                    {
                        if (currentConv.Dialogs[i] != null && currentConv.Dialogs[i].windowRect.Contains(mousePos))
                        {
                            clickedOnWindow = true;
                            selectedNodeIndex = i;

                            GenericMenu menu = new GenericMenu();
                            if (currentConv.Dialogs[i].charac.IsPlayable)
                                menu.AddItem(new GUIContent("Add choice"), false, AddChoiceOnNode, i);
                            menu.AddItem(new GUIContent("Add link"), false, SpeakMenu, "makeLink");
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Delete dialog"), false, SpeakMenu, "deleteNode");
                            menu.ShowAsContext();
                            e.Use();
                            break;
                        }
                    }
                }

                // Or click on a link
                bool clickedOnLink = false;
                if (clickedOnTransition == false && clickedOnWindow == false)
                {

                    for (int i = 0; i < links.Count; i++)
                    {
                        if (links[i] != null && links[i].linkRect.Contains(mousePos))
                        {
                            clickedOnLink = true;

                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Remove link"), false, RemoveLink, links[i]);
                            menu.ShowAsContext();
                            e.Use();
                        }
                    }
                }

                // OR click elsewhere
                if (clickedOnTransition == false && clickedOnWindow == false && clickedOnLink == false)
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < characters.Count; i++)
                    {
                        menu.AddItem(new GUIContent("Add speech/" + characters[i].Name), false, CreateDialogNode, characters[i]);
                    }
                    for (int i = 0; i < characters.Count; i++)
                    {
                        if (characters[i].IsPlayable)
                            menu.AddItem(new GUIContent("Add choice/" + characters[i].Name), false, CreateChoiceNode, characters[i]);
                    }
                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }

        /// <summary>
        /// Left click events
        /// </summary>
        private void HandleLeftClick()
        {
            // Click on a link : display infos
            bool clickedOnLink = false;
            for (int i = 0; i < links.Count; i++)
            {
                if (links[i] != null && links[i].linkRect.Contains(mousePos))
                {
                    clickedOnLink = true;
                    if (selectedLinkIndex != -1)
                        links[selectedLinkIndex].IsUnselected();
                    links[i].IsSelected();
                    selectedLinkIndex = i;
                    tempLink = null;
                }
            }
            // If click elsewhere : reinit selected link infos
            if (clickedOnLink == false && selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].IsUnselected();
                selectedLinkIndex = -1;
                currentCondition = 0;
            }

            // OR click to begin/end drawing a new link
            if (clickedOnLink == false)
            {
                if (tempLink == null)
                    BeginDrawingLink();
                else
                    EndDrawingLink();
            }

            // Or click on a node : selection
            bool clickedOnWindow = false;
            if (clickedOnLink == false)
            {
                for (int i = 0; i < currentConv.Dialogs.Count; i++)
                {
                    if (currentConv.Dialogs[i] != null && currentConv.Dialogs[i].windowRect.Contains(mousePos) == true)
                    {
                        clickedOnWindow = true;
                        selectedNodeIndex = i;
                        break;
                    }
                }
                if (clickedOnWindow == false && currentConv.Entry.windowRect.Contains(mousePos) == true)
                {
                    clickedOnWindow = true;
                    selectedNodeIndex = -1;
                }
            }
        }

        /// <summary>
        /// Handle background drag
        /// </summary>
        /// <param name="_delta"></param>
        private void OnDrag(Vector2 _delta)
        {
            backDrag += _delta;
            currentConv.Entry.windowRect.position += _delta * 0.5f;
            for (int i = 0; i < currentConv.Dialogs.Count; i++)
            {
                currentConv.Dialogs[i].windowRect.position += _delta * 0.5f;
            }
            Repaint();
        }


        //________________ DISPLAY : WINDOW BACKGROUND ________________//

        /// <summary>
        /// Display window background (color & grid)
        /// </summary>
        private void DrawWindowBackground()
        {
            Color color = Color.grey;
            color.r = 0.30f;
            color.g = 0.30f;
            color.b = 0.30f;

            GUI.backgroundColor = color;
            backgroundRect.width = position.width;
            backgroundRect.height = position.height;
            GUI.Box(backgroundRect, GUIContent.none);
            DrawGrid(15, 0.1f, Color.white);
            DrawGrid(150, 0.6f, Color.black);
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Display window background grid (called in DrawWindowBackground)
        /// </summary>
        /// <param name="gridSpacing"></param>
        /// <param name="gridOpacity"></param>
        /// <param name="gridColor"></param>
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
            backOffset = backDrag * 0.5f;
            Vector3 newOffset = new Vector3(backOffset.x % gridSpacing, backOffset.y % gridSpacing, 0);
            for (int i = 0; i <= widthDivs; i++)
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + newOffset);
            for (int j = 0; j <= heightDivs; j++)
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + newOffset);
            Handles.color = Color.white;
            Handles.EndGUI();
        }


        //________________ DISPLAY : LEFT WINDOW (CONVERSATIONS) ________________//

        /// <summary>
        /// Called once to initialize conversation window
        /// </summary>
        void InitializeLeftWindow_Conversations()
        {
            convOrLang[0] = "Conversations";
            convOrLang[1] = "Langages";

            LoadAllConversations();

            conversationsNames = new string[conversationList.Count];
            for (int i = 0; i < conversationList.Count; i++)
            {
                conversationsNames[i] = conversationList[i].ConversationName;
            }
        }

        /// <summary>
        /// Display left window up : switch between conversations & langages
        /// </summary>
        /// <param name="_id"></param>
        void DrawLeftWindow_ConvAndLangages(int _id)
        {
            switch(convOrLangages = GUI.Toolbar(charOrParamsRect, convOrLangages, convOrLang))
            {
                case 0:
                    DrawLeftWindow_Conversations();
                    break;
                case 1:
                    DrawLeftWindow_Langages();
                    break;
            }
        }


        /// <summary>
        /// Display all conversations and select current
        /// </summary>
        /// <param name="_id"></param>
        void DrawLeftWindow_Conversations()
        {
            if (conversationList.Count != 0)
            {
                int tempIndex = currentConversationIndex;
                convListScrollVect = EditorGUILayout.BeginScrollView(convListScrollVect, GUILayout.Width(leftWindow_Up.width - 10), GUILayout.Height(leftWindow_Up.height - 30));
                GUILayout.BeginHorizontal();
                currentConversationIndex = GUILayout.SelectionGrid(currentConversationIndex, conversationsNames, 1);
                GUILayout.EndHorizontal();

                if (GUILayout.Button("+", GUILayout.Width(leftWindow_Up.width - 20)))
                {
                    CreateConversation("");
                }
                if (tempIndex != currentConversationIndex)
                {
                    UpdateCurrentConversation();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Space(10.0f);
                if (GUILayout.Button("+", GUILayout.Width(leftWindow_Up.width - 20)))
                {
                    CreateConversation("");
                }
            }
        }

        /// <summary>
        /// Display langages and select current
        /// </summary>
        void DrawLeftWindow_Langages()
        {
            convListScrollVect = EditorGUILayout.BeginScrollView(convListScrollVect, GUILayout.Width(leftWindow_Up.width - 10), GUILayout.Height(leftWindow_Up.height - 30));

            brain.CurrentLanguageIndex = GUILayout.SelectionGrid(brain.CurrentLanguageIndex, brain.GetLanguagesArray(), 1);

            if (GUILayout.Button("+", GUILayout.Width(leftWindow_Up.width - 20)))
            {
                brain.AddLanguage();
            }            

            EditorGUILayout.EndScrollView();

            // Input Handle
            if (leftWindow_Up.Contains(mousePos) && Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Rename"), false, RenameLanguageBegin);
                menu.AddItem(new GUIContent("Delete"), false, DeleteLangage);

                menu.ShowAsContext();
            }

        }

        /// <summary>
        /// Update to call whenever selected conversation changes
        /// </summary>
        void UpdateCurrentConversation()
        {
            if (conversationList.Count != 0 && conversationList[currentConversationIndex] != null)
            {
                // Nodes
                currentConv = conversationList[currentConversationIndex];

                // Links
                UpdateLinks();
            }

            // characters : distinguish playable & non-playable characters on display
            characters.Clear();
            for (int i = 0; i < brain.NPCs.Count; i++)
                characters.Add(brain.NPCs[i]);
            for (int i = 0; i < brain.PCs.Count; i++)
                characters.Add(brain.PCs[i]);
        }


        void RenameConversation()
        {
            //EditorGUI.Popup()
        }

        void DeleteConversation()
        {
            // deleting conversation
        }

        void RenameLanguageBegin()
        {
            NarratorPopUp.Init(brain.CurrentLanguage, new Vector2(position.x, position.y) + mousePos);
        }

        void RenameLanguageEnd()
        {
            brain.RenameCurrentLanguage(NarratorPopUp.GetContent());
            NarratorPopUp.ClosePopUp();
        }


        void DeleteLangage()
        {
            if (EditorUtility.DisplayDialog("Langage suppression", "Are you sure you want to delete " + brain.CurrentLanguage + " language ?\nAll the content in this language will be deleted.", "Yes", "No"))
            {
                for (int i = 0; i < conversationList.Count; i++)
                {
                    for (int d = 0; d < conversationList[i].Dialogs.Count; d++)
                    {
                        conversationList[i].Dialogs[d].DeleteLanguageContent(brain.CurrentLanguageIndex);
                    }
                }
                brain.DeleteCurrentLanguage();
            }

        }



        //________________ DISPLAY : LEFT WINDOW (CHARACTERS & PARAMETERS) ________________//

        /// <summary>
        /// Called once to initialize Character and Parameters left window
        /// </summary>
        void InitializeLeftWindow_CharAndParams()
        {
            charOrParams[0] = "Characters";
            charOrParams[1] = "Parameters";
        }

        /// <summary>
        /// Display characters & parameters left window
        /// </summary>
        /// <param name="_id"></param>
        void DrawLeftWindow_CharAndParams(int _id)
        {
            leftWindow_Down.height = position.height * 0.6f;

            switch (charOrParamsIndex = GUI.Toolbar(charOrParamsRect, charOrParamsIndex, charOrParams))
            {
                case 0:
                    DrawCharactersList();
                    break;
                case 1:
                    DrawParametersList();
                    break;
            }
        }

        /// <summary>
        /// Display characters list
        /// </summary>
        void DrawCharactersList()
        {
            if (brain != null)
            {
                EditorGUILayout.LabelField("Player(s)");
                playersListScrollVec = EditorGUILayout.BeginScrollView(playersListScrollVec, GUILayout.Width(leftWindow_Down.width -10), GUILayout.Height(leftWindow_Down.height * 0.4f - 5.0f));
                int sup = -1;
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].IsPlayable == true)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("x"))
                        {
                            sup = i;
                            i = characters.Count;
                        }
                        else
                        {
                            string name  = EditorGUILayout.TextField(characters[i].Name);
                            characters[i].Color = EditorGUILayout.ColorField(characters[i].Color);

                            if(name != characters[i].Name)
                            {
                                characters[i].Name = name;
                                brain.SaveCharacterList();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                if (sup != -1)
                {
                    if (EditorUtility.DisplayDialog("Character suppression", "Are you sure you want to delete " + characters[sup].Name + " ?\nAll his/her nodes will be deleted.", "Yes", "No"))
                        DeleteCharacter(characters[sup]);
                }
                if (GUILayout.Button("+", GUILayout.Width(leftWindow_Up.width - 20)))
                {
                    CreateCharacter(true);
                }
                GUILayout.EndScrollView();

                EditorGUILayout.LabelField("Character(s)");
                npcsListScrollVec = EditorGUILayout.BeginScrollView(npcsListScrollVec, GUILayout.Width(leftWindow_Down.width -10), GUILayout.Height(leftWindow_Down.height * 0.4f - 5.0f));

                sup = -1;
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].IsPlayable == false)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("x"))
                        {
                            sup = i;
                            i = characters.Count;
                        }
                        else
                        {
                            characters[i].Name = EditorGUILayout.TextField(characters[i].Name);
                            characters[i].Color = EditorGUILayout.ColorField(characters[i].Color);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                if (sup != -1)
                {
                    if (EditorUtility.DisplayDialog("Delete character ?", "Are you sure you want to delete " + characters[sup].Name + " ?\nAll his/her nodes will be deleted.", "Yes", "No"))
                        DeleteCharacter(characters[sup]);
                }
                if (GUILayout.Button("+", GUILayout.Width(leftWindow_Up.width - 20)))
                {
                    CreateCharacter(false);
                }
                GUILayout.EndScrollView();
            }
            else
                Debug.LogError("Can't display characters : conversation brain is null");
        }

        /// <summary>
        /// Display parameters list
        /// </summary>
        void DrawParametersList()
        {
            if (brain != null)
            {
                string exKey = string.Empty;
                string newKey = string.Empty;
                float newFValue = 0.0f;
                int newIValue = 0;
                bool newBValue = false;
                bool isDeleting = false;

                EditorGUILayout.LabelField("Float");
                paramsListScrollVec[0] = EditorGUILayout.BeginScrollView(paramsListScrollVec[0], GUILayout.Width(leftWindow_Down.width - 10), GUILayout.Height(leftWindow_Down.height / 5));                
                foreach (string str in brain.Parameters.FloatValues.dictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    newKey = EditorGUILayout.TextField(str);
                    if (newKey != str)
                    {
                        exKey = str;
                        newFValue = EditorGUILayout.FloatField(brain.Parameters.FloatValues.dictionary[str]);
                        break;
                    }
                    newFValue = EditorGUILayout.FloatField(brain.Parameters.FloatValues.dictionary[str]);
                    if (GUILayout.Button("x"))
                    {
                        isDeleting = true;
                        exKey = str;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                brain.Parameters.SaveFloatModifications(exKey, newKey, newFValue, isDeleting);
                GUILayout.EndScrollView();

                EditorGUILayout.LabelField("Int");
                exKey = "";
                newKey = "";
                isDeleting = false;
                paramsListScrollVec[1] = EditorGUILayout.BeginScrollView(paramsListScrollVec[1], GUILayout.Width(leftWindow_Down.width - 10), GUILayout.Height(leftWindow_Down.height / 5));
                foreach (string str in brain.Parameters.IntValues.dictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    newKey = EditorGUILayout.TextField(str);
                    if (newKey != str)
                    {
                        exKey = str;
                        newIValue = EditorGUILayout.IntField(brain.Parameters.IntValues.dictionary[str]);
                        break;
                    }
                    newIValue = EditorGUILayout.IntField(brain.Parameters.IntValues.dictionary[str]);
                    if (GUILayout.Button("x"))
                    {
                        isDeleting = true;
                        exKey = str;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                brain.Parameters.SaveIntModifications(exKey, newKey, newIValue, isDeleting);
                GUILayout.EndScrollView();

                EditorGUILayout.LabelField("Bool");
                exKey = "";
                newKey = "";
                isDeleting = false;
                paramsListScrollVec[2] = EditorGUILayout.BeginScrollView(paramsListScrollVec[2], GUILayout.Width(leftWindow_Down.width - 10), GUILayout.Height(leftWindow_Down.height / 5));
                foreach (string str in brain.Parameters.BoolValues.dictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    newKey = EditorGUILayout.TextField(str);
                    if (newKey != str)
                    {
                        exKey = str;
                        newBValue = EditorGUILayout.Toggle(brain.Parameters.BoolValues.dictionary[str]);
                        break;
                    }
                    newBValue = EditorGUILayout.Toggle(brain.Parameters.BoolValues.dictionary[str]);
                    if (GUILayout.Button("x"))
                    {
                        isDeleting = true;
                        exKey = str;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                brain.Parameters.SaveBoolModifications(exKey, newKey, newBValue, isDeleting);
                GUILayout.EndScrollView();
            }
            else
                Debug.LogError("Can't display parameters : conversation brain is null");
        }

         
        //______________ MODIFICATIONS : NARRATOR BRAIN _______________//

        /// <summary>
        /// Add a new parameter to the Conversation Brain
        /// </summary>
        /// <param name="_obj"></param>
        void CreateParameter(object _obj)
        {
            Parameters.TYPE type = (Parameters.TYPE)_obj;
            switch (type)
            {
                case Parameters.TYPE.f:
                    brain.Parameters.AddFloat();
                    break;
                case Parameters.TYPE.b:
                    brain.Parameters.AddBool();
                    break;
                case Parameters.TYPE.i:
                    brain.Parameters.AddInt();
                    break;
            }

            EditorUtility.SetDirty(brain);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Add a new character to the Conversation Brain
        /// </summary>
        /// <param name="_obj"></param>
        void CreateCharacter(object _obj)
        {
            bool isPlayable = (bool)_obj;
            Character charac = new Character("new character ", isPlayable, Color.white);
            int index = 0;
            bool isNameOk = false;
            while (isNameOk == false)
            {
                isNameOk = true;
                charac.Name = "new character " + index;
                index++;
                for (int i = 0; i < characters.Count; i++)
                    if (charac.Name == characters[i].Name)
                        isNameOk = false;
            }
            characters.Add(charac);
            brain.AddCharacter(charac);

            EditorUtility.SetDirty(brain);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Delete a character in the Conversation Brain (and the characters list of NarratorWindow)
        /// </summary>
        /// <param name="_char"></param>
        void DeleteCharacter(Character _char)
        {
            characters.Remove(_char);
            brain.DeleteCharacter(_char);

            for(int i = 0; i < conversationList.Count; i++)
            {
                bool nodeHasBeenDeleted = true;
                while (nodeHasBeenDeleted == true)
                {
                    nodeHasBeenDeleted = false;
                    for (int n = 0; n < conversationList[i].GetDialogsCount; n++)
                    {
                        if (conversationList[i].Dialogs[n].charac == _char)
                        {
                            conversationList[i].DeleteNodeFromDialog(conversationList[i].Dialogs[n]);
                            nodeHasBeenDeleted = true;
                            break;
                        }
                    }
                }
            }

            UpdateLinks();
        }


        //________________ NODES ________________//

        /// <summary>
        /// Add a new dialog node to the Conversation
        /// </summary>
        /// <param Node Character="_character"></param>
        void CreateDialogNode(object _character)
        {
            Character charac = (Character)_character;
            SpeakNode newNode = new SpeakNode();
            newNode.CreateSpeakNode(currentConv.Dialogs.Count, brain);
            newNode.charac = charac;
            newNode.position = new Vector2(mousePos.x, mousePos.y);
            newNode.windowRect = new Rect(newNode.position.x, newNode.position.y, 200, 100);

            currentConv.AddDialogNode(newNode);

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Add a new choice node to the Conversation
        /// </summary>
        /// <param Node Character="_character"></param>
        void CreateChoiceNode(object _character)
        {
            Character charac = (Character)_character;
            SpeakNode newNode = new SpeakNode();
            newNode.CreateSpeakNode(currentConv.Dialogs.Count, 2, brain);
            newNode.charac = charac;
            newNode.position = new Vector2(mousePos.x, mousePos.y);
            newNode.windowRect = new Rect(newNode.position.x, newNode.position.y, 200, 100);

            currentConv.AddDialogNode(newNode);

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Add a choice content on an existing node
        /// </summary>
        /// <param name="_nodeIndex"></param>
        void AddChoiceOnNode(object _nodeIndex)
        {
            Content content = new Content();
            content.texts[brain.CurrentLanguageIndex] = "new choice";
            content.Initialize(brain);

            if ((int)_nodeIndex >= 0 && (int)_nodeIndex < currentConv.Dialogs.Count)
                currentConv.Dialogs[(int)_nodeIndex].contents.Add(content);
            else
                Debug.LogError("Fail to add choice on node at index " + (int)_nodeIndex);
        }

        /// <summary>
        /// Display an existing speak node
        /// </summary>
        /// <param name="_id"></param>
        void DrawSpeakNode(int _id)
        {
            currentConv.Dialogs[_id - (int)windowID.dialogs].DrawWindow(brain.CurrentLanguageIndex);
            GUI.DragWindow();
        }

        /// <summary>
        /// Display the existing entry node
        /// </summary>
        /// <param name="_id"></param>
        void DrawEntryNode(int _id)
        {
            GUI.DragWindow();
        }

        /// <summary>
        /// Delete an existing node and all the links related to it
        /// </summary>
        /// <param name="_nodeIndex"></param>
        void DeleteNode(int _nodeIndex)
        {
            if (selectedNodeIndex >= 0 && selectedLinkIndex < currentConv.Dialogs.Count)
            {
                // Remove links related to the node
                bool searchingLinks = true;
                while (searchingLinks)
                {
                    searchingLinks = false;
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (links[i].end == currentConv.Dialogs[_nodeIndex] || links[i].start == currentConv.Dialogs[_nodeIndex])
                        {
                            links.RemoveAt(i);
                            searchingLinks = true;
                            break;
                        }
                    }
                }
                // Delete node
                currentConv.DeleteNodeFromDialog(currentConv.Dialogs[_nodeIndex]);
                selectedNodeIndex = 0;
            }
            else
                Debug.Log("Can't delete node number " + _nodeIndex + ": it doesn't exist");
        }

        /// <summary>
        /// Menu displayed when a node is right-clicked on
        /// </summary>
        /// <param name="_obj"></param>
        void SpeakMenu(object _obj)
        {
            switch (_obj.ToString())
            {
                case "makeLink":
                    Node node = new Node();
                    if (selectedNodeIndex == -1)
                        node = currentConv.Entry;
                    else
                        node = currentConv.Dialogs[selectedNodeIndex];
                    tempLink = new Link(node, 0, linkColor);
                    break;
                case "deleteNode":
                    DeleteNode(selectedNodeIndex);
                    break;
                default:
                    Debug.LogError("Can't display the menu on this dialog window : case unknown");
                    break;
            }
        }


        //________________ LINKS ________________//

        /// <summary>
        /// Update the links displayed in the NarratorWindow accroding to the current conversation
        /// </summary>
        void UpdateLinks()
        {
            links.Clear();
            Link link;

            // Entry node's links
            for (int i = 0; i < currentConv.Entry.contents[0].nextNodes.Count; i++)
            {
                link = new Link(currentConv.Entry, 0, currentConv.Dialogs[currentConv.Entry.contents[0].nextNodes[i].index - 1], linkColor);
                link.startBoxIndex = 0;
                link.nextNodeIndex = i + 1;
                for (int k = 0; k < currentConv.Entry.contents[0].nextNodes[i].conditions.Count; k++)
                {
                    link.conditions.Add(currentConv.Entry.contents[0].nextNodes[i].conditions[k]);
                }
                for (int k = 0; k < currentConv.Entry.contents[0].nextNodes[i].impacts.Count; k++)
                {
                    link.impacts.Add(currentConv.Entry.contents[0].nextNodes[i].impacts[k]);
                }
                links.Add(link);
            }

            // Dialog nodes' links
            for (int i = 0; i < currentConv.Dialogs.Count; i++)
            {
                // For each exit box
                for (int j = 0; j < currentConv.Dialogs[i].contents.Count; j++)
                {
                    // Initialize all the links
                    for (int k = 0; k < currentConv.Dialogs[i].contents[j].nextNodes.Count; k++)
                    {
                        link = new Link(currentConv.Dialogs[i], j, currentConv.Dialogs[currentConv.Dialogs[i].contents[j].nextNodes[k].index - 1], linkColor);
                        link.startBoxIndex = j;
                        link.nextNodeIndex = k;
                        // For each link : initialize conditions
                        for (int l = 0; l < currentConv.Dialogs[i].contents[j].nextNodes[k].conditions.Count; l++)
                        {
                            link.conditions.Add(currentConv.Dialogs[i].contents[j].nextNodes[k].conditions[l]);
                        }
                        // For each link : initialize impacts
                        for (int l = 0; l < currentConv.Dialogs[i].contents[j].nextNodes[k].impacts.Count; l++)
                        {
                            link.impacts.Add(currentConv.Dialogs[i].contents[j].nextNodes[k].impacts[l]);
                        }
                        links.Add(link);
                    }
                }

            }
        }

        /// <summary>
        /// Create a link begining on the selected exit box
        /// </summary>
        void BeginDrawingLink()
        {
            if (tempLink == null)
            {
                // The link begins on the entry node
                if (currentConv.Entry.contents[0].exitBox.Contains(mousePos))
                {
                    selectedNodeIndex = -1;
                    tempLink = new Link(currentConv.Entry, 0, linkColor);
                }

                // OR on a dialog node
                else
                {
                    for (int i = 0; i < currentConv.Dialogs.Count; i++)
                    {
                        for (int j = 0; j < currentConv.Dialogs[i].contents.Count; j++)
                        {
                            if (currentConv.Dialogs[i].contents[j].exitBox.Contains(mousePos))
                            {
                                selectedNodeIndex = i;
                                tempLink = new Link(currentConv.Dialogs[i], j, linkColor);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// End the tracing of the link on the selected entry box and save it
        /// </summary>
        void EndDrawingLink()
        {
            for (int i = 0; i < currentConv.Dialogs.Count; i++)
            {
                if (currentConv.Dialogs[i].entryBox.Contains(mousePos))
                {
                    tempLink.EndTrace(currentConv.Dialogs[i], conversationList[currentConversationIndex]);
                    links.Add(tempLink);
                    break;
                }
            }
            tempLink = null;

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }


        /// <summary>
        /// Delete an existing link and save modifications
        /// </summary>
        /// <param name="_link"></param>
        void RemoveLink(object _link)
        {
            Link link = _link as Link;
            currentConv.DeleteLinkFromDialog(link.start, link.end, link.startBoxIndex);
            links.Remove(link);

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        //__________SELECTED LINK______________//

        /// <summary>
        /// Initialization : called once when Narrator Window opens
        /// </summary>
        void SelectedLink_Initialize()
        {
            selectedLinkIndex = -1;
        }

        /// <summary>
        /// Add an empty condition on selected link
        /// </summary>
        void AddConditionOnLink()
        {
            if (selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].AddCondition();
                conversationList[currentConversationIndex].AddCondition(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, new Condition());
            }
        }

        /// <summary>
        /// Add an empty impact on selected link
        /// </summary>
        void AddImpactOnLink()
        {
            if (selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].AddImpact();
                conversationList[currentConversationIndex].AddImpact(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, new Impact());
            }
        }

        /// <summary>
        /// Display the selected link, manage and save modifications
        /// </summary>
        /// <param name="_id"></param>
        void DrawSelectedLink(int _id)
        {
            // Display all link's conditions
            GUILayout.Label("Conditions");
            Condition cond = new Condition();

            for (int i = 0; i < links[selectedLinkIndex].conditions.Count; i++)
            {
                currentCondition = i;
                cond = links[selectedLinkIndex].conditions[i];

                GUILayout.BeginHorizontal();

                // Condition's name (the parameter it refers to)
                if (EditorGUILayout.DropdownButton(new GUIContent(cond.name), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < brain.Parameters.Names.Count; j++)
                    {
                        menu.AddItem(new GUIContent(brain.Parameters.Names[j]), false, UpdateLink_Condition_Parameter, new Vector2(i, j));
                    }
                    menu.ShowAsContext();
                }

                // Condition's operator (equals, inferior, etc.)
                switch (links[selectedLinkIndex].conditions[i].type)
                {
                    case Parameters.TYPE.b:
                        break;
                    case Parameters.TYPE.f:
                        if (EditorGUILayout.DropdownButton(new GUIContent(cond.test.ToString()), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("less"), false, UpdateLink_Condition_Operator, Parameters.OPERATOR.less);
                            menu.AddItem(new GUIContent("greater"), false, UpdateLink_Condition_Operator, Parameters.OPERATOR.greater);
                            menu.ShowAsContext();
                        }
                        break;
                    case Parameters.TYPE.i:
                        if (EditorGUILayout.DropdownButton(new GUIContent(cond.test.ToString()), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("equals"), false, UpdateLink_Condition_Operator, Parameters.OPERATOR.equals);
                            menu.AddItem(new GUIContent("less"), false, UpdateLink_Condition_Operator, Parameters.OPERATOR.less);
                            menu.AddItem(new GUIContent("greater"), false, UpdateLink_Condition_Operator, Parameters.OPERATOR.greater);
                            menu.ShowAsContext();
                        }
                        break;
                    default:
                        Debug.LogError("Can't display condition, unknown type");
                        break;

                }

                // Condition's value (the value the parameters will be compared with)
                string marker = "marker";
                switch (links[selectedLinkIndex].conditions[i].type)
                {
                    case Parameters.TYPE.b:
                        marker = cond.boolMarker == true ? "true" : "false";
                        if (EditorGUILayout.DropdownButton(new GUIContent(marker), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("true"), false, UpdateLink_Condition_Marker_bool, true);
                            menu.AddItem(new GUIContent("false"), false, UpdateLink_Condition_Marker_bool, false);
                            menu.ShowAsContext();
                        }
                        break;
                    case Parameters.TYPE.f:
                        float fValue = EditorGUILayout.FloatField(cond.floatMarker);
                        if (cond.floatMarker < fValue - 0.05f || cond.floatMarker > fValue + 0.05f)
                            UpdateLink_Condition_Marker_float(fValue);
                        break;
                    case Parameters.TYPE.i:
                        int iValue = EditorGUILayout.IntField(cond.intMarker);
                        if (cond.intMarker != iValue)
                            UpdateLink_Condition_Marker_int(iValue);
                        break;
                    default:
                        marker = "error";
                        break;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(1.0f);

            // Display all link's impacts
            GUILayout.Label("Impacts");
            Impact imp = new Impact();

            for (int i = 0; i < links[selectedLinkIndex].impacts.Count; i++)
            {
                imp = links[selectedLinkIndex].impacts[i];
                GUILayout.BeginHorizontal();

                // Impact's name : the parameter it refers to
                if (EditorGUILayout.DropdownButton(new GUIContent(imp.name), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < brain.Parameters.Names.Count; j++)
                    {
                        menu.AddItem(new GUIContent(brain.Parameters.Names[j]), false, UpdateLink_Impact_Parameter, new Vector2(i, j));
                    }
                    menu.ShowAsContext();
                }

                // Impact's value : how will it modifies the parameters
                switch (imp.type)
                {
                    case Parameters.TYPE.b:
                        string boolStr = imp.boolModifier == true ? "true" : "false";
                        if (EditorGUILayout.DropdownButton(new GUIContent(boolStr), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("true"), false, UpdateLink_Impact_Modifier_boolTrue, i);
                            menu.AddItem(new GUIContent("false"), false, UpdateLink_Impact_Modifier_boolFalse, i);
                            menu.ShowAsContext();
                        }
                        break;
                    case Parameters.TYPE.f:
                        float fValue = EditorGUILayout.FloatField(imp.floatModifier);
                        if (imp.floatModifier < fValue - 0.05f || imp.floatModifier > fValue + 0.05f)
                            UpdateLink_Impact_Modifier_floatOrint(i, fValue);
                        break;
                    case Parameters.TYPE.i:
                        int iValue = EditorGUILayout.IntField(imp.intModifier);
                        if (imp.intModifier != iValue)
                            UpdateLink_Impact_Modifier_floatOrint(i, iValue);
                        break;
                    default:
                        break;
                }

                GUILayout.EndHorizontal();
            }
        }


        /// <summary>
        /// Update the condition's parameter (_obj is a Vector2, x = condition index, y = new parameter index)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Condition_Parameter(object _obj)
        {
            Vector2 index = (Vector2)_obj;

            links[selectedLinkIndex].conditions[(int)index.x].name = brain.Parameters.Names[(int)index.y];
            links[selectedLinkIndex].conditions[(int)index.x].type = brain.Parameters.GetType(links[selectedLinkIndex].conditions[(int)index.x].name);

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, nextNodeIndex, (int)index.x, links[selectedLinkIndex].conditions[(int)index.x]);
        }

        /// <summary>
        /// Update the condition's operator (_obj is the new Parameters.OPERATOR)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Condition_Operator(object _obj)
        {
            Parameters.OPERATOR op = (Parameters.OPERATOR)_obj;

            links[selectedLinkIndex].conditions[currentCondition].test = op;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        /// <summary>
        /// Update the condition's bool marker value (_obj is the new bool value)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Condition_Marker_bool(object _obj)
        {
            bool value = (bool)_obj;

            links[selectedLinkIndex].conditions[currentCondition].boolMarker = value;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        /// <summary>
        /// Update the condition's int marker value (_obj is the new int value)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Condition_Marker_int(object _obj)
        {
            int value = (int)_obj;

            links[selectedLinkIndex].conditions[currentCondition].intMarker = value;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        /// <summary>
        /// Update the condition's float marker value (_obj is the new float value)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Condition_Marker_float(object _obj)
        {
            float value = (float)_obj;

            links[selectedLinkIndex].conditions[currentCondition].floatMarker = value;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        /// <summary>
        /// Update the impact's parameter (_obj is a Vector2, x = impact index, y = parameter index)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Impact_Parameter(object _obj)
        {
            Vector2 index = (Vector2)_obj;

            links[selectedLinkIndex].impacts[(int)index.x].name = brain.Parameters.Names[(int)index.y];
            links[selectedLinkIndex].impacts[(int)index.x].type = brain.Parameters.GetType(links[selectedLinkIndex].impacts[(int)index.x].name);

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, nextNodeIndex, (int)index.x, links[selectedLinkIndex].impacts[(int)index.x]);
        }

        /// <summary>
        /// Update the impact's float or int value (_obj is the new int of float value)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Impact_Modifier_floatOrint(int _index, float _value)
        {
            links[selectedLinkIndex].impacts[_index].floatModifier = _value;
            links[selectedLinkIndex].impacts[_index].intModifier = (int)_value;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, nextNodeIndex, _index, links[selectedLinkIndex].impacts[_index]);
        }

        /// <summary>
        /// Set the impact's bool value to true (_obj is the impact index)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Impact_Modifier_boolTrue(object _obj)
        {
            int index = (int)_obj;
            links[selectedLinkIndex].impacts[index].boolModifier = true;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, nextNodeIndex, currentCondition, links[selectedLinkIndex].impacts[index]);
        }

        /// <summary>
        /// Set the impact's bool value to false (_obj is the impact index)
        /// </summary>
        /// <param name="_obj"></param>
        void UpdateLink_Impact_Modifier_boolFalse(object _obj)
        {
            int index = (int)_obj;
            links[selectedLinkIndex].impacts[index].boolModifier = true;

            int nextNodeIndex = (links[selectedLinkIndex].start.type == Node.Type.entry) ? links[selectedLinkIndex].nextNodeIndex - 1 : links[selectedLinkIndex].nextNodeIndex;
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, nextNodeIndex, currentCondition, links[selectedLinkIndex].impacts[index]);
        }
    

        //____________LOAD & SAVE DATA__________//

        /// <summary>
        /// Create a new conversation (in Assets folder) and add it to conversations list
        /// </summary>
        /// <param name="_obj"></param>
        void CreateConversation(object _obj)
        {
            // création de la nouvelle conv dans les assets
            ConversationSO newConv = CreateInstance<ConversationSO>();
            newConv.CreateConversation();

            string outPath = EditorUtility.SaveFilePanel("Select output file", Application.dataPath, "new Conversation.asset", "asset");
            if (outPath == string.Empty)
                return;
            outPath = outPath.Substring(outPath.IndexOf("Assets/"));
            newConv.ConversationName = Path.GetFileNameWithoutExtension(outPath);
            AssetDatabase.CreateAsset(newConv, @outPath);

            // link de l'asset vers la fenetre Narrator
            conversationList.Add(newConv);
            string[] tempNames = conversationsNames;
            conversationsNames = new string[conversationsNames.Length + 1];
            for (int i = 0; i < tempNames.Length; i++)
            {
                conversationsNames[i] = tempNames[i];
            }
            conversationsNames[tempNames.Length] = newConv.ConversationName;

            UpdateCurrentConversation();

            EditorUtility.SetDirty(newConv);
            AssetDatabase.SaveAssets();

        }

        /// <summary>
        /// Get all the ConversationSO in Resources folder of the project
        /// </summary>
        void LoadAllConversations()
        {
            string[] guids = AssetDatabase.FindAssets("t:ConversationSO", null);
            conversationList = new List<ConversationSO>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                ConversationSO newConv = AssetDatabase.LoadAssetAtPath(path, typeof(ConversationSO)) as ConversationSO;

                Debug.Assert(newConv != null, "Failed loading conversation at : " + path);
                conversationList.Add(newConv);
            }
        }

        /// <summary>
        /// Get the ConversationBrainSO in Resources folder of the project
        /// </summary>
        void LoadConversationBrain()
        {
            string [] guid = AssetDatabase.FindAssets("t:NarratorBrainSO", null);

            // Load the brain if there is one in Assets folder
            if (guid.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid[0]);
                brain = AssetDatabase.LoadAssetAtPath(path, typeof(NarratorBrainSO)) as NarratorBrainSO;

                Debug.Assert(brain != null, "Could not load brain at : " + path);
            }
            // Generate a brain if there is none
            else if (guid.Length == 0)
            {
                brain = CreateInstance<NarratorBrainSO>();
                AssetDatabase.CreateAsset(brain, "Assets/NarratorBrain.asset");
            }
            // Narrator can't handle more than two brains for the moment : the first encountered is taken, others are ignored
            else
            { 
                Debug.LogWarning("Narrator can't handle more than two brains, the first one encounter has been loaded, other(s) will be ignored");

                string path = AssetDatabase.GUIDToAssetPath(guid[0]);
                brain = AssetDatabase.LoadAssetAtPath(path, typeof(NarratorBrainSO)) as NarratorBrainSO;

                Debug.Assert(brain != null, "Could not load brain at : " + path);
            }

            if (brain.NPCs == null)
                brain.CreateBrain();
        }


    }// end class



}//end namespace
