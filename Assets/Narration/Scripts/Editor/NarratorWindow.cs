/* NARRATOR PACKAGE
 * NarratorEditor.cs
 * Created by Ambre Lacour, 05/10/2017
 * Editor script : Display & edit conversations
 * 
 * The Narrator window :
 *      - loads all the ConversationSO it can find in resources folders
 *      - creates conversations
 *      - edits conversations i.e. :
 *             =>create/edit dialogs,
 *             =>create/edit links between dialogs
 *             =>add/remove characters
 *             =>add/remove parameters & conditions to manage the narration tree
 */


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


namespace Narrator
{
    public class NarratorWindow : EditorWindow
    {
        enum windowID
        {
            conversations,
            characters,
            entryNode,
            dialogs,
            transitions,
        }

        private Vector2 mousePos;
        private float zoomValue = 1.0f;
        private Vector2 zoomPos = Vector2.one;
        private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        private Vector2 scrollStartMousePos;


        // Left window
            // Conversations list
        Rect leftWindow_Up = new Rect(0.0f, 0.0f, 200.0f, 200.0f);
        Rect addConvRect = new Rect(170.0f, 0.0f, 20.0f, 15.0f);
        Rect convSelectionRect = new Rect(25, 30, 150, 0);
        List<ConversationSO> conversationList;
        string[] conversationsNames;
        int currentConversationIndex = 0;
            // Characters & params list
        Rect leftWindow_Down = new Rect(0.0f, 200.0f, 200.0f, 200.0f);
        Rect charOrParamsRect = new Rect(0.0f, 0.0f, 200.0f, 20.0f);
        int charOrParamsIndex = 0;
        string[] charOrParams = new string[2];
        List<Character> characters = new List<Character>();
        Parameters parameters;


        // Main window
            // Background
        private Rect backgroundRect = new Rect(0.0f, 0.0f, 800.0f, 800.0f);
        private Vector2 backOffset;
        private Vector2 backDrag;
        // Dialogs
        private ConversationSO currentConv;
        private int selectedNodeIndex;
            // Links
        private List<Link> links = new List<Link>();
        private Link tempLink;
        private Color linkColor = Color.white;


        //Menu item
        [MenuItem("Window/Narrator")]
        static void ShowEditor()
        {
            // Création de la fenêtre
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
            InitializeLeftWindow_Conversations();
            InitializeLeftWindow_CharAndParams();
            currentConv = CreateInstance<ConversationSO>();
            currentConv.CreateConversation();
            UpdateCurrentConversation();
        }


        private void Update()
        {
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
            leftWindow_Up = GUI.Window((int)windowID.conversations, leftWindow_Up, DrawLeftWindow_Conversations, "Conversations");
            leftWindow_Down = GUI.Window((int)windowID.characters, leftWindow_Down, DrawLeftWindow_CharAndParams, "");



            // Display all links
            if (tempLink != null)
                tempLink.Draw(mousePos);
            for (int i = 0; i < links.Count; i++)
            {
                links[i].Draw(mousePos);
                GUI.Box(links[i].linkRect, "");
                for(int j=0; j < links[i].conditions.Count; j++)
                {
                    // draw condition
                }
            }

            // Display all nodes
            if (conversationList.Count != 0)
            {
                if (currentConv.Entry != null)
                {
                    currentConv.Entry.windowRect = GUI.Window((int)windowID.entryNode, currentConv.Entry.windowRect, DrawEntryNode, "Entry");
                    currentConv.Entry.DrawBox();
                }
                for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
                {
                    if (currentConv.Dialogs.dictionary[i] != null)
                    {
                        GUI.backgroundColor = currentConv.Dialogs.dictionary[i].charac.Color;
                        currentConv.Dialogs.dictionary[i].windowRect = GUI.Window(i + (int)windowID.dialogs, currentConv.Dialogs.dictionary[i].windowRect, DrawSpeakNode, currentConv.Dialogs.dictionary[i].charac.Name);
                        currentConv.Dialogs.dictionary[i].DrawBox();
                        GUI.backgroundColor = Color.white;
                    }
                }
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
            if (convSelectionRect.Contains(mousePos))
            {
                // Clic sur la liste des conversation
            }
            else if (leftWindow_Up.Contains(mousePos))
            {
                // Clic sur la fenêtre de gauche
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add conversation"), false, CreateConversation, "createConv");
                menu.ShowAsContext();
                e.Use();
            }
            else if (leftWindow_Down.Contains(mousePos))
            {
                // Clic sur la fenêtre de gauche (persos et parametres)
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
            else
            {
                bool clickedOnWindow = false;
                // clic sur l'un des dialogue sur la fenêtre principale
                for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
                {
                    if (currentConv.Dialogs.dictionary[i] != null && currentConv.Dialogs.dictionary[i].windowRect.Contains(mousePos))
                    {

                        // Faire un truc
                        clickedOnWindow = true;
                        selectedNodeIndex = i;

                        GenericMenu menu = new GenericMenu();
                        if(currentConv.Dialogs.dictionary[i].charac.IsPlayable)
                            menu.AddItem(new GUIContent("Add choice"), false, AddChoiceOnNode, i);
                        menu.AddItem(new GUIContent("Add link"), false, SpeakMenu, "makeLink");
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Delete dialog"), false, SpeakMenu, "deleteNode");

                        //we use it so that it will show
                        menu.ShowAsContext();
                        //consumes the event
                        e.Use();
                        break;
                    }
                }

                bool clickedOnLink = false;
                if (clickedOnWindow == false)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (links[i] != null && links[i].linkRect.Contains(mousePos))
                        {
                            clickedOnLink = true;

                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add condition"), false, AddParameterOnLink, links[i]);
                            menu.AddItem(new GUIContent("Remove link"), false, RemoveLink, links[i]);

                            menu.ShowAsContext();
                            e.Use();
                        }
                    }
                }

                // clic ailleurs
                if (clickedOnWindow == false && clickedOnLink == false)
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
            if (tempLink == null)
                BeginDrawingLink();
            else
                EndDrawingLink();
        }

        /// <summary>
        /// Handle background drag
        /// </summary>
        /// <param name="_delta"></param>
        private void OnDrag(Vector2 _delta)
        {
            backDrag += _delta;
            currentConv.Entry.windowRect.position += _delta * 0.5f;
            for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
            {
                currentConv.Dialogs.dictionary[i].windowRect.position += _delta * 0.5f;
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
            DrawGrid(10, 0.2f, Color.black);
            DrawGrid(100, 0.4f, Color.black);
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
            LoadAllConversations();

            conversationsNames = new string[conversationList.Count];
            for (int i = 0; i < conversationList.Count; i++)
            {
                conversationsNames[i] = conversationList[i].ConversationName;
            }
        }

        /// <summary>
        /// Display conversation window
        /// </summary>
        /// <param name="_id"></param>
        void DrawLeftWindow_Conversations(int _id)
        {
            // Create conversation button
            if (GUI.Button(addConvRect, "+"))
            {
                CreateConversation("");
            }

            // Conversations list display
            if (conversationList.Count != 0)
            {
                int tempIndex = currentConversationIndex;
                convSelectionRect.height = conversationsNames.Length * 25;
                currentConversationIndex =  GUI.SelectionGrid(convSelectionRect, currentConversationIndex, conversationsNames, 1);
                
                if(tempIndex != currentConversationIndex)
                    UpdateCurrentConversation();
            }
        }

        /// <summary>
        /// Update to call whenever selected conversation changes
        /// </summary>
        void UpdateCurrentConversation()
        {
            if (conversationList.Count != 0 && conversationList[currentConversationIndex] != null)
            {
                currentConv = conversationList[currentConversationIndex];

                links.Clear();

                // Links
                for (int i = 0; i < currentConv.Entry.contents[0].nextNodes.Count; i++)
                {
                    links.Add(new Link(currentConv.Entry, 0, conversationList[currentConversationIndex].Dialogs.dictionary[currentConv.Entry.contents[0].nextNodes[i].index], linkColor));
                }

                // Pour chaque dialogue
                for (int i = 1; i <= conversationList[currentConversationIndex].Dialogs.dictionary.Count; i++)
                {
                    // Pour chaque boxe de sortie
                    for (int j = 0; j < currentConv.Dialogs.dictionary[i].contents.Count; j ++)
                    {
                        // Récupération de tous les liens
                        for (int k = 0; k < currentConv.Dialogs.dictionary[i].contents[j].nextNodes.Count; k++)
                        {
                            links.Add(new Link(currentConv.Dialogs.dictionary[i], j, conversationList[currentConversationIndex].Dialogs.dictionary[currentConv.Dialogs.dictionary[i].contents[j].nextNodes[k].index], linkColor));
                        }
                    }

                }

                // characters
                characters.Clear();
                for (int i = 0; i < conversationList[currentConversationIndex].NPCs.Count; i++)
                    characters.Add(conversationList[currentConversationIndex].NPCs[i]);
                for (int i = 0; i < conversationList[currentConversationIndex].PCs.Count; i++)
                    characters.Add(conversationList[currentConversationIndex].PCs[i]);

                // parameters
                parameters = conversationList[currentConversationIndex].Parameters;
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
            parameters = new Parameters();
            if(conversationList.Count > 0)
                parameters = conversationList[currentConversationIndex].Parameters;
        }

        /// <summary>
        /// Display characters & parameters left window
        /// </summary>
        /// <param name="_id"></param>
        void DrawLeftWindow_CharAndParams(int _id)
        {
                     
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
            if(conversationList.Count > 0)
            {
                EditorGUILayout.LabelField("Player(s)");
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].IsPlayable == true)
                    {
                        GUILayout.BeginHorizontal();
                        characters[i].Name = EditorGUILayout.TextField(characters[i].Name);
                        characters[i].Color = EditorGUILayout.ColorField(characters[i].Color);
                        GUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.LabelField("Character(s)");
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].IsPlayable == false)
                    {
                        GUILayout.BeginHorizontal();
                        characters[i].Name = EditorGUILayout.TextField(characters[i].Name);
                        characters[i].Color = EditorGUILayout.ColorField(characters[i].Color);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        /// <summary>
        /// Display parameters list
        /// </summary>
        void DrawParametersList()
        {
            if (conversationList.Count > 0)
            {
                string exKey = string.Empty;
                string newKey = string.Empty;
                float newFValue = 0.0f;
                int newIValue = 0;
                bool newBValue = false;
                bool isDeleting = false;

                EditorGUILayout.LabelField("Float");

                foreach (string str in parameters.FloatValues.dictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    newKey = EditorGUILayout.TextField(str);
                    if (newKey != str)
                    {
                        exKey = str;
                        newFValue = EditorGUILayout.FloatField(parameters.FloatValues.dictionary[str]);
                        break;
                    }
                    newFValue = EditorGUILayout.FloatField(parameters.FloatValues.dictionary[str]);
                    if (GUILayout.Button("x"))
                    {
                        isDeleting = true;
                        exKey = str;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                //SaveFloatModifications(exKey, newKey, newFValue, isDeleting);


                EditorGUILayout.LabelField("Int");
                exKey = "";
                newKey = "";
                isDeleting = false;
                foreach (string str in parameters.IntValues.dictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    newKey = EditorGUILayout.TextField(str);
                    if (newKey != str)
                    {
                        exKey = str;
                        newIValue = EditorGUILayout.IntField(parameters.IntValues.dictionary[str]);
                        break;
                    }
                    newIValue = EditorGUILayout.IntField(parameters.IntValues.dictionary[str]);
                    if (GUILayout.Button("x"))
                    {
                        isDeleting = true;
                        exKey = str;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                //SaveIntModifications(exKey, newKey, newIValue, isDeleting);

                EditorGUILayout.LabelField("Bool");
                exKey = "";
                newKey = "";
                isDeleting = false;
                foreach (string str in parameters.BoolValues.dictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    newKey = EditorGUILayout.TextField(str);
                    if (newKey != str)
                    {
                        exKey = str;
                        newBValue = EditorGUILayout.Toggle(parameters.BoolValues.dictionary[str]);
                        break;
                    }
                    newBValue = EditorGUILayout.Toggle(parameters.BoolValues.dictionary[str]);
                    if (GUILayout.Button("x"))
                    {
                        isDeleting = true;
                        exKey = str;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                //SaveBoolModifications(exKey, newKey, newBValue, isDeleting);
            }
        }


        //________________ MODIFICATIONS : PARAMETERS ________________//

        void CreateParameter(object _obj)
        {
            Parameters.TYPE type = (Parameters.TYPE)_obj;
            switch (type)
            {
                case Parameters.TYPE.f:
                    parameters.AddFloat();
                    break;
                case Parameters.TYPE.b:
                    parameters.AddBool();
                    break;
                case Parameters.TYPE.i:
                    parameters.AddInt();
                    break;
            }
            conversationList[currentConversationIndex].Parameters = parameters;

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        void SaveFloatModifications(string _exKey, string _newKey, float _newValue, bool _isDeleting)
        {
            if (_isDeleting && parameters.FloatValues.dictionary.ContainsKey(_exKey))
            {
                parameters.FloatValues.dictionary.Remove(_exKey);
                conversationList[currentConversationIndex].Parameters = parameters;
            }
            else
            {
                if (_exKey != string.Empty && parameters.FloatValues.dictionary.ContainsKey(_newKey) == false)
                {
                    parameters.FloatValues.dictionary.Remove(_exKey);
                    parameters.FloatValues.dictionary.Add(_newKey, _newValue);
                    conversationList[currentConversationIndex].Parameters = parameters;
                }
                else if (parameters.FloatValues.dictionary.ContainsKey(_newKey) && _newValue != parameters.FloatValues.dictionary[_newKey])
                {
                    parameters.FloatValues.dictionary[_newKey] = _newValue;
                    conversationList[currentConversationIndex].Parameters = parameters;
                }
            }
        }

        void SaveIntModifications(string _exKey, string _newKey, int _newValue, bool _isDeleting)
        {
            if (_isDeleting && parameters.IntValues.dictionary.ContainsKey(_exKey))
            {
                parameters.IntValues.dictionary.Remove(_exKey);
                conversationList[currentConversationIndex].Parameters = parameters;
            }
            else
            {
                if (_exKey != string.Empty && parameters.IntValues.dictionary.ContainsKey(_newKey) == false)
                {
                    parameters.IntValues.dictionary.Remove(_exKey);
                    parameters.IntValues.dictionary.Add(_newKey, _newValue);
                    conversationList[currentConversationIndex].Parameters = parameters;
                }
                else if (parameters.IntValues.dictionary.ContainsKey(_newKey) && _newValue != parameters.IntValues.dictionary[_newKey])
                {
                    parameters.IntValues.dictionary[_newKey] = _newValue;
                    conversationList[currentConversationIndex].Parameters = parameters;
                }
            }
        }

        void SaveBoolModifications(string _exKey, string _newKey, bool _newValue, bool _isDeleting)
        {
            if (_isDeleting && parameters.BoolValues.dictionary.ContainsKey(_exKey))
            {
                parameters.BoolValues.dictionary.Remove(_exKey);
                conversationList[currentConversationIndex].Parameters = parameters;
            }
            else
            {
                if (_exKey != string.Empty && parameters.BoolValues.dictionary.ContainsKey(_newKey) == false)
                {
                    parameters.BoolValues.dictionary.Remove(_exKey);
                    parameters.BoolValues.dictionary.Add(_newKey, _newValue);
                    conversationList[currentConversationIndex].Parameters = parameters;
                }
                else if (parameters.BoolValues.dictionary.ContainsKey(_newKey) && _newValue != parameters.BoolValues.dictionary[_newKey])
                {
                    parameters.BoolValues.dictionary[_newKey] = _newValue;
                    conversationList[currentConversationIndex].Parameters = parameters;
                }
            }
        }


        //________________ MODIFICATIONS : CHARACTERS ________________//

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
            conversationList[currentConversationIndex].AddCharacter(charac);

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }


        //________________ NODES ________________//
        void CreateDialogNode(object _obj)
        {
            Character charac = (Character)_obj;
            SpeakNode newNode = new SpeakNode();
            newNode.CreateSpeakNode();
            newNode.charac = charac;
            newNode.position = new Vector2(mousePos.x, mousePos.y);
            newNode.windowRect = new Rect(newNode.position.x, newNode.position.y, 200, 100);

            currentConv.AddDialogNode(newNode);

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        void CreateChoiceNode(object _obj)
        {
            Character charac = (Character)_obj;
            SpeakNode newNode = new SpeakNode();
            newNode.CreateSpeakNode(2);
            newNode.charac = charac;
            newNode.position = new Vector2(mousePos.x, mousePos.y);
            newNode.windowRect = new Rect(newNode.position.x, newNode.position.y, 200, 100);

            currentConv.AddDialogNode(newNode);

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        void AddChoiceOnNode(object _nodeIndex)
        {
            Content content = new Content();
            content.text = "new choice";
            content.Initialize();

            int index = (int)_nodeIndex;
            currentConv.Dialogs.dictionary[index].contents.Add(content);
        }

        void DrawSpeakNode(int _id)
        {
            currentConv.Dialogs.dictionary[_id - (int)windowID.dialogs].DrawWindow();
            GUI.DragWindow();
        }

        void DrawEntryNode(int _id)
        {
            GUI.DragWindow();
        }

        void DeleteNode(int _nodeIndex)
        {
            if (selectedNodeIndex > 0)
            {
                Node node = currentConv.Dialogs.dictionary[_nodeIndex];
                //Supression des liens entrants et sortants du noeud
                bool searchingLinks = true;
                while (searchingLinks)
                {
                    searchingLinks = false;
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (links[i].end == node || links[i].start == node)
                        {
                            links.RemoveAt(i);
                            searchingLinks = true;
                            break;
                        }
                    }
                }

                // Supression du noeud
                currentConv.Dialogs.dictionary.Remove(selectedNodeIndex);
            }

            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        void SpeakMenu(object _obj)
        {
            switch(_obj.ToString())
            {
                case "makeLink":
                    Node node = new Node();
                    if (selectedNodeIndex == 0)
                        node = currentConv.Entry;
                    else
                        node = currentConv.Dialogs.dictionary[selectedNodeIndex];
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

        void BeginDrawingLink()
        {
            if (tempLink == null)
            {
                if (currentConv.Entry.contents[0].exitBox.Contains(mousePos))
                {
                    selectedNodeIndex = 0;
                    tempLink = new Link(currentConv.Entry, 0, linkColor);
                }
                else
                {
                    for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
                    {
                        for (int j = 0; j < currentConv.Dialogs.dictionary[i].contents.Count; j++)
                        {
                            if (currentConv.Dialogs.dictionary[i].contents[j].exitBox.Contains(mousePos))
                            {
                                selectedNodeIndex = i;
                                tempLink = new Link(currentConv.Dialogs.dictionary[selectedNodeIndex], j, linkColor);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void EndDrawingLink()
        {
            for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
            {
                if (currentConv.Dialogs.dictionary[i].entryBox.Contains(mousePos))
                {
                    tempLink.EndTrace(currentConv.Dialogs.dictionary[i], conversationList[currentConversationIndex]);
                    links.Add(tempLink);
                    tempLink.Save(currentConv);
                    break;
                }
            }
            tempLink = null;
            EditorUtility.SetDirty(currentConv);
            AssetDatabase.SaveAssets();
        }

        void AddParameterOnLink(object _link)
        {
            Link link = (Link)_link;
            Condition condition = new Condition();
            //condition.
            //link.conditions.Add()
        }

        void RemoveLink(object _link)
        {
            Link link = _link as Link;
            currentConv.DeleteLinkFromDialog(link.start, link.end, link.startBoxIndex);
            links.Remove(link);
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
            Debug.Log("Create " + newConv.ConversationName + " conversation in assetdatabase");
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
                path = path.Replace(".asset", string.Empty);
                path = path.Substring(path.IndexOf("Resources") + "Resources/".Length);
                ConversationSO newConv = Resources.Load<ConversationSO>(path);
                if (newConv == null)
                    Debug.LogError("Could not load conversation at : " + path + "\n Is conversation asset in a Resources folder ?");
                else
                {
                    conversationList.Add(newConv);
                    Debug.Log("conv : " + newConv.ConversationName + " loaded from assetdatabase");
                }
            }
        }

    }// end class



}//end namespace
