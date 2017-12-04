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
            transitions,
            entryNode,
            dialogs,
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
        // Conversation brain : Characters & params list
        NarratorBrainSO brain;
        Rect leftWindow_Down = new Rect(0.0f, 200.0f, 200.0f, 200.0f);
        Rect charOrParamsRect = new Rect(0.0f, 0.0f, 200.0f, 20.0f);
        int charOrParamsIndex = 0;
        string[] charOrParams = new string[2];
        List<Character> characters = new List<Character>();


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
        //private int currentImpact = 0;



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

            LoadConversationBrain();

            InitializeLeftWindow_Conversations();
            InitializeLeftWindow_CharAndParams();
            SelectedLink_Initialize();

            currentConv = CreateInstance<ConversationSO>();
            currentConv.CreateConversation();
            UpdateCurrentConversation();
        }


        private void Update()
        {
            //Debug.Log("selected node index : " + selectedNodeIndex);
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

            // Display selected link infos
            if(selectedLinkIndex != -1)
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
                // clic sur les infos d'une transition
                bool clickedOnTransition = false;
                if(selectedLinkIndex != -1)
                {
                    if(links[selectedLinkIndex].linkRect.Contains(mousePos))
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
                // OU clic sur l'un des noeuds de dialogue
                bool clickedOnWindow = false;
                if (clickedOnTransition == false)
                {
                    for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
                    {
                        if (currentConv.Dialogs.dictionary[i] != null && currentConv.Dialogs.dictionary[i].windowRect.Contains(mousePos))
                        {

                            // Faire un truc
                            clickedOnWindow = true;
                            selectedNodeIndex = currentConv.Dialogs.dictionary[i].ID;

                            GenericMenu menu = new GenericMenu();
                            if (currentConv.Dialogs.dictionary[i].charac.IsPlayable)
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

                // OU clic sur l'un des liens
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

                // OU clic ailleurs
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
            // Affichage des infos d'un lien
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
            if (clickedOnLink == false && selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].IsUnselected();
                selectedLinkIndex = -1;
                currentCondition = 0;
            }

            // OU dessin d'un lien
            if (clickedOnLink == false)
            {
                if (tempLink == null)
                    BeginDrawingLink();
                else
                    EndDrawingLink();
            }

            // OU sélection d'un node
            bool clickedOnWindow = false;
            if (clickedOnLink == false)
            {
                for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
                {
                    if (currentConv.Dialogs.dictionary[i] != null && currentConv.Dialogs.dictionary[i].windowRect.Contains(mousePos) == true)
                    {
                        clickedOnWindow = true;
                        selectedNodeIndex = currentConv.Dialogs.dictionary[i].ID;
                        break;
                    }
                }
                if(clickedOnWindow == false && currentConv.Entry.windowRect.Contains(mousePos) == true)
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
                Link link;
                for (int i = 0; i < currentConv.Entry.contents[0].nextNodes.Count; i++)
                {
                    link = new Link(currentConv.Entry, 0, currentConv.Dialogs.dictionary[currentConv.Entry.contents[0].nextNodes[i].index], linkColor);
                    link.startBoxIndex = 0;
                    link.nextNodeIndex = i;
                    for(int k = 0; k < currentConv.Entry.contents[0].nextNodes[i].conditions.Count; k++)
                    {
                        link.conditions.Add(currentConv.Entry.contents[0].nextNodes[i].conditions[k]);
                    }
                    for (int k = 0; k < currentConv.Entry.contents[0].nextNodes[i].impacts.Count; k++)
                    {
                        link.impacts.Add(currentConv.Entry.contents[0].nextNodes[i].impacts[k]);
                    }
                    links.Add(link);
                }

                // Pour chaque dialogue
                for (int i = 1; i <= currentConv.Dialogs.dictionary.Count; i++)
                {
                    // Pour chaque boxe de sortie
                    for (int j = 0; j < currentConv.Dialogs.dictionary[i].contents.Count; j ++)
                    {
                        // Récupération de tous les liens
                        for (int k = 0; k < currentConv.Dialogs.dictionary[i].contents[j].nextNodes.Count; k++)
                        {
                            link = new Link(currentConv.Dialogs.dictionary[i], j, currentConv.Dialogs.dictionary[currentConv.Dialogs.dictionary[i].contents[j].nextNodes[k].index], linkColor);
                            link.startBoxIndex = j;
                            link.nextNodeIndex = k;
                            // pour chaque lien : conditions
                            for (int l = 0; l < currentConv.Dialogs.dictionary[i].contents[j].nextNodes[k].conditions.Count; l++)
                            {
                                link.conditions.Add(currentConv.Dialogs.dictionary[i].contents[j].nextNodes[k].conditions[l]);
                            }
                            // pour chaque lien : impacts
                            for (int l = 0; l < currentConv.Dialogs.dictionary[i].contents[j].nextNodes[k].impacts.Count; l++)
                            {
                                link.impacts.Add(currentConv.Dialogs.dictionary[i].contents[j].nextNodes[k].impacts[l]);
                            }
                            links.Add(link);
                        }
                    }

                }
            }

            // characters
            characters.Clear();
            for (int i = 0; i < brain.NPCs.Count; i++)
                characters.Add(brain.NPCs[i]);
            for (int i = 0; i < brain.PCs.Count; i++)
                characters.Add(brain.PCs[i]);
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

        /// <summary>
        /// Display parameters list
        /// </summary>
        void DrawParametersList()
        {

            string exKey = string.Empty;
            string newKey = string.Empty;
            float newFValue = 0.0f;
            int newIValue = 0;
            bool newBValue = false;
            bool isDeleting = false;

            EditorGUILayout.LabelField("Float");
            if (brain != null)
            {
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
                //brain.Parameters.SaveFloatModifications(exKey, newKey, newFValue, isDeleting);
            }

            EditorGUILayout.LabelField("Int");
            exKey = "";
            newKey = "";
            isDeleting = false;
            if (brain != null)
            {
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
            }

            EditorGUILayout.LabelField("Bool");
            exKey = "";
            newKey = "";
            isDeleting = false;
            if (brain != null)
            {
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
            }
        }


        //________________ MODIFICATIONS : PARAMETERS ________________//

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
            brain.AddCharacter(charac);

            EditorUtility.SetDirty(brain);
            AssetDatabase.SaveAssets();
        }


        //________________ NODES ________________//
        void CreateDialogNode(object _obj)
        {
            Character charac = (Character)_obj;
            SpeakNode newNode = new SpeakNode();
            newNode.CreateSpeakNode(currentConv.Dialogs.dictionary.Count);
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
            newNode.CreateSpeakNode(currentConv.Dialogs.dictionary.Count, 2);
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
                    if (selectedNodeIndex == -1)
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
                    selectedNodeIndex = -1;
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
                                selectedNodeIndex = currentConv.Dialogs.dictionary[i].ID;
                                tempLink = new Link(currentConv.Dialogs.dictionary[i], j, linkColor);
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

        //__________SELECTED LINK______________//

        void SelectedLink_Initialize()
        {
            selectedLinkIndex = -1;
        }

        void AddConditionOnLink()
        {
            if(selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].AddCondition();
                conversationList[currentConversationIndex].AddCondition(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, new Condition());
            }
        }

        void AddImpactOnLink()
        {
            if(selectedLinkIndex != -1)
            {
                links[selectedLinkIndex].AddImpact();
                conversationList[currentConversationIndex].AddImpact(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, new Impact());
            }
        }

        void DrawSelectedLink(int _id)
        {
            GUILayout.Label("Conditions");
            Condition cond = new Condition();
            // Affichage de chaque condition posée sur le liens
            for (int i = 0; i < links[selectedLinkIndex].conditions.Count; i++)
            {
                currentCondition = i;
                cond = links[selectedLinkIndex].conditions[i];
                GUILayout.BeginHorizontal();
                // paramètre de la condition
                if (EditorGUILayout.DropdownButton(new GUIContent(cond.name), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < brain.Parameters.Names.Count; j++)
                    {
                        menu.AddItem(new GUIContent(brain.Parameters.Names[j]), false, UpdateSelectedLinkParameter, j);
                    }
                    menu.ShowAsContext();
                }
                // operateur de la condition
                switch (links[selectedLinkIndex].conditions[i].type)
                {
                    case Parameters.TYPE.b:
                        break;
                    case Parameters.TYPE.f:
                        if (EditorGUILayout.DropdownButton(new GUIContent(cond.test.ToString()), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("less"), false, UpdateSelectedLinkOperator, Parameters.OPERATOR.less);
                            menu.AddItem(new GUIContent("greater"), false, UpdateSelectedLinkOperator, Parameters.OPERATOR.greater);
                            menu.ShowAsContext();
                        }
                        break;
                    case Parameters.TYPE.i:
                        if (EditorGUILayout.DropdownButton(new GUIContent(cond.test.ToString()), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("equals"), false, UpdateSelectedLinkOperator, Parameters.OPERATOR.equals);
                            menu.AddItem(new GUIContent("less"), false, UpdateSelectedLinkOperator, Parameters.OPERATOR.less);
                            menu.AddItem(new GUIContent("greater"), false, UpdateSelectedLinkOperator, Parameters.OPERATOR.greater);
                            menu.ShowAsContext();
                        }
                        break;
                    default:
                        Debug.LogError("Can't display condition, unknown type");
                        break;

                }
                // markeur de la condition
                string marker = "marker";
                switch (links[selectedLinkIndex].conditions[i].type)
                {
                    case Parameters.TYPE.b:
                        marker = cond.boolMarker == true ? "true" : "false";
                        if (EditorGUILayout.DropdownButton(new GUIContent(marker), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("true"), false, UpdateSelectedLinkMarker_bool, true);
                            menu.AddItem(new GUIContent("false"), false, UpdateSelectedLinkMarker_bool, false);
                            menu.ShowAsContext();
                        }
                        break;
                    case Parameters.TYPE.f:
                        float fValue = EditorGUILayout.FloatField(cond.floatMarker);
                        //if (Mathf.Approximately(cond.floatMarker, fValue))
                        //    UpdateSelectedLinkMarker_float(fValue);
                        break;
                    case Parameters.TYPE.i:
                        int iValue = EditorGUILayout.IntField(cond.intMarker);
                        if (cond.intMarker != iValue)
                            UpdateSelectedLinkMarker_int(iValue);
                        break;
                    default:
                        marker = "error";
                        break;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(1.0f);
            GUILayout.Label("Impacts");
            Impact imp = new Impact();
            // Affichage de chaque impact qu'entrainera le lien une fois franchi
            for (int i = 0; i < links[selectedLinkIndex].impacts.Count; i++)
            {
                imp = links[selectedLinkIndex].impacts[i];
                GUILayout.BeginHorizontal();
                // paramètre de l'impact
                if (EditorGUILayout.DropdownButton(new GUIContent(imp.name), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < brain.Parameters.Names.Count; j++)
                    {
                        menu.AddItem(new GUIContent(brain.Parameters.Names[j]), false, UpdateSelectedLinkImpact_Parameter, new Vector2(i, j));
                    }
                    menu.ShowAsContext();
                }
                // valeur de l'impact
                switch (links[selectedLinkIndex].impacts[i].type)
                {
                    case Parameters.TYPE.b:
                        string boolStr = imp.boolModifier == true ? "true" : "false";
                        if (EditorGUILayout.DropdownButton(new GUIContent(boolStr), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("true"), false, UpdateSelectedLinkMarkerImpact_boolTrue, i);
                            menu.AddItem(new GUIContent("false"), false, UpdateSelectedLinkMarkerImpact_boolFalse, i);
                            menu.ShowAsContext();
                        }
                        break;
                    case Parameters.TYPE.f:
                        float fValue = EditorGUILayout.FloatField(imp.floatModifier);
                        //if (Mathf.Approximately(imp.floatModifier, fValue))
                        //    UpdateSelectedLinkImpact(fValue);
                        break;
                    case Parameters.TYPE.i:
                        int iValue = EditorGUILayout.IntField(imp.intModifier);
                        if (imp.intModifier != iValue)
                            UpdateSelectedLinkImpact_Marker(i, iValue);
                        break;
                    default:
                        break;
                }
                GUILayout.EndHorizontal();
            }
        }

        void UpdateSelectedLinkParameter(object _obj)
        {
            int index = (int)_obj;

            links[selectedLinkIndex].conditions[currentCondition].name = brain.Parameters.Names[index];
            links[selectedLinkIndex].conditions[currentCondition].type = brain.Parameters.GetType(links[selectedLinkIndex].conditions[currentCondition].name);
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);

        }

        void UpdateSelectedLinkOperator(object _obj)
        {
            Parameters.OPERATOR op = (Parameters.OPERATOR)_obj;

            links[selectedLinkIndex].conditions[currentCondition].test = op;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        void UpdateSelectedLinkMarker_bool(object _obj)
        {
            bool value = (bool)_obj;

            links[selectedLinkIndex].conditions[currentCondition].boolMarker = value;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        void UpdateSelectedLinkMarker_int(object _obj)
        {
            int value = (int)_obj;

            links[selectedLinkIndex].conditions[currentCondition].intMarker = value;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }

        void UpdateSelectedLinkMarker_float(object _obj)
        {
            float value = (float)_obj;

            links[selectedLinkIndex].conditions[currentCondition].floatMarker = value;
            conversationList[currentConversationIndex].UpdateCondition(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].conditions[currentCondition]);
        }


        void UpdateSelectedLinkImpact_Parameter(object _obj)
        {
            Vector2 index = (Vector2)_obj;

            links[selectedLinkIndex].impacts[(int)index.x].name = brain.Parameters.Names[(int)index.y];
            links[selectedLinkIndex].impacts[(int)index.x].type = brain.Parameters.GetType(links[selectedLinkIndex].impacts[(int)index.x].name);
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, (int)index.x, links[selectedLinkIndex].impacts[(int)index.x]);
        }

        void UpdateSelectedLinkImpact_Marker(int _index, float _value)
        {
            links[selectedLinkIndex].impacts[_index].floatModifier = _value;
            links[selectedLinkIndex].impacts[_index].intModifier = (int)_value;

            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, _index, links[selectedLinkIndex].impacts[_index]);
        }

        void UpdateSelectedLinkMarkerImpact_boolTrue(object _obj)
        {
            int index = (int)_obj;
            links[selectedLinkIndex].impacts[index].boolModifier = true;
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].impacts[index]);
        }

        void UpdateSelectedLinkMarkerImpact_boolFalse(object _obj)
        {
            int index = (int)_obj;
            links[selectedLinkIndex].impacts[index].boolModifier = true;
            conversationList[currentConversationIndex].UpdateImpact(links[selectedLinkIndex].start, (int)links[selectedLinkIndex].startBoxIndex, links[selectedLinkIndex].nextNodeIndex, currentCondition, links[selectedLinkIndex].impacts[index]);
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

                if (brain == null)
                    Debug.LogError("Could not load brain at : " + path);
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

                if (brain == null)
                    Debug.LogError("Could not load brain at : " + path);
            }

            if (brain.NPCs == null)
                brain.CreateBrain();
        }


    }// end class



}//end namespace
