using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Narrator;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        menu,
        menuFadingOut,
        gameIntro,
        game,
        gameEnd,
        menuFadingIn
    }
    [SerializeField]private GameState state = GameState.menu;
    private bool initState = false;

    [Header("Menu")]
    [SerializeField] private GameObject menu;
    private float menuTransition;
    [SerializeField] [Range(0.0f, 1.0f)]private float transSpeed = 0.5f;

    [Header("Game")]
    [SerializeField] private NarratorBrainSO brain;
    [SerializeField] private ConversationUI convUI;
    [SerializeField] private GameObject sliders;
    [SerializeField] private ConversationSO introConv;

    [SerializeField] private int peopleCount = 3;
    [SerializeField] private ConversationSO[] mumConversations;
    [SerializeField] private ConversationSO[] dadConversations;
    [SerializeField] private ConversationSO[] bossConversations;
    [SerializeField] private ConversationSO[] doctorConversations;

    [SerializeField] private ConversationSO[] conscienceConversations;
    [SerializeField] private ConversationSO[] homelessConversations;
    [SerializeField] private ConversationSO[] landlordConversations;

    [SerializeField] private ConversationSO[] dogConversations;
    [SerializeField] private ConversationSO[] coworkerConversations;


    [Header("End Game")]
    [SerializeField] private GameObject endGame;

    public static int score = 0;


    // Use this for initialization
    void Start()
    {
        GoBackToMenu();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case GameState.menuFadingIn:
                MenuFadeIn();
                break;
            case GameState.menu:
                break;
            case GameState.menuFadingOut:
                MenuFadeOut();
                break;
            case GameState.gameIntro:
                GameIntro();
                break;
            case GameState.game:
                Game();
                break;
            case GameState.gameEnd:
                EndGame();
                break;
        }

        if(brain.Parameters.GetBool("slidersOn") == true && sliders.activeInHierarchy == false)
        {
            sliders.SetActive(true);
        }

    }


    public void Play()
    {
        state = GameState.menuFadingOut;
        menuTransition = 1.0f;
    }



    public void RestartGame()
    {
        convUI.gameObject.SetActive(false);
        sliders.SetActive(false);
        endGame.SetActive(false);
        menu.SetActive(false);

        InitBrain();
        state = GameState.gameIntro;
        initState = false;
    }

    public void GoBackToMenu()
    {
        InitBrain();
        convUI.gameObject.SetActive(false);
        sliders.SetActive(false);
        endGame.SetActive(false);
        menu.SetActive(true);

        menuTransition = 0.0f;
        state = GameState.menuFadingIn;
        initState = false;

        score = 0;
    }

    void InitBrain()
    {
        brain.Parameters.SetInt("love", 50);
        brain.Parameters.SetInt("ego", 50);
        brain.Parameters.SetInt("energy", 50);
        brain.Parameters.SetInt("money", 50);

        brain.Parameters.SetBool("slidersOn", false);

    }

    //___________________________STATE UPDATE_________________________//
    void MenuFadeOut()
    {
        menuTransition -= Time.deltaTime * transSpeed;
        if (menuTransition < 0.0f)
        {
            state = GameState.gameIntro;
            menu.SetActive(false);
            return;
        }
        foreach (Text t in menu.GetComponentsInChildren<Text>())
        {
            t.color = new Color(t.color.r, t.color.g, t.color.b, menuTransition);
        }
        foreach (Image i in menu.GetComponentsInChildren<Image>())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, menuTransition);
        }
    }

    void MenuFadeIn()
    {
        menuTransition += Time.deltaTime * transSpeed;
        if (menuTransition > 1.0f)
        {
            state = GameState.menu;
            return;
        }
        foreach (Text t in menu.GetComponentsInChildren<Text>())
        {
            t.color = new Color(t.color.r, t.color.g, t.color.b, menuTransition);
        }
        foreach (Image i in menu.GetComponentsInChildren<Image>())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, menuTransition);
        }
    }


    void GameIntro()
    {
        if(initState == false)
        {
            convUI.NewConversation(introConv);
            convUI.gameObject.SetActive(true);
            initState = true;
        }
        else if(convUI.IsOver)
        {
            state = GameState.game;
            initState = false;
        }
    }


    void Game()
    {
        if (initState == false)
        {
            int nextPeople = Random.Range(0, peopleCount);
            switch(nextPeople)
            {
                case 0:
                    Debug.Log("launch mum conv");
                    convUI.NewConversation(PickSomeoneConv(mumConversations));
                    break;
                case 1:
                    Debug.Log("launch dad conv");
                    convUI.NewConversation(PickSomeoneConv(dadConversations));
                    break;
                case 2:
                    Debug.Log("launch boss conv");
                    convUI.NewConversation(PickSomeoneConv(bossConversations));
                    break;
                case 3:
                    Debug.Log("launch conscience conv");
                    convUI.NewConversation(PickSomeoneConv(doctorConversations));
                    break;
                case 4:
                    Debug.Log("launch conscience conv");
                    convUI.NewConversation(PickSomeoneConv(conscienceConversations));
                    break;
                case 5:
                    Debug.Log("launch conscience conv");
                    convUI.NewConversation(PickSomeoneConv(homelessConversations));
                    break;
                case 6:
                    Debug.Log("launch conscience conv");
                    convUI.NewConversation(PickSomeoneConv(landlordConversations));
                    break;
                case 7:
                    Debug.Log("launch conscience conv");
                    convUI.NewConversation(PickSomeoneConv(dogConversations));
                    break;
                case 8:
                    Debug.Log("launch conscience conv");
                    convUI.NewConversation(PickSomeoneConv(coworkerConversations));
                    break;
            }
            
            convUI.gameObject.SetActive(true);
            initState = true;
        }
        else if (convUI.IsOver)
        {
            score++;
            state = GameState.game;
            initState = false;
        }

        if (AsLost() == true)
        {
            state = GameState.gameEnd;
            initState = false;
        }
    }

    void EndGame()
    {
        if (initState == false)
        {
            convUI.gameObject.SetActive(false);
            brain.Parameters.SetBool("slidersOn", false);
            sliders.SetActive(false);

            endGame.GetComponent<ConversationUI>().Init();
            endGame.SetActive(true);
            initState = true;
        }            
    }

    ConversationSO PickSomeoneConv(ConversationSO[] _conv)
    {
        return _conv[Random.Range(0, _conv.Length)];
    }


    bool AsLost()
    {
        return brain.Parameters.GetInt("money") <= 0 || brain.Parameters.GetInt("money") >= 100
            || brain.Parameters.GetInt("love") <= 0 || brain.Parameters.GetInt("love") >= 100
            || brain.Parameters.GetInt("ego") <= 0 || brain.Parameters.GetInt("ego") >= 100
            || brain.Parameters.GetInt("energy") <= 0 || brain.Parameters.GetInt("energy") >= 100;
    }

}

