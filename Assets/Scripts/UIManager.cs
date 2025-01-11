using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Статический экземпляр синглтона
    public static UIManager Instance { get; private set; }

    public BattleGameSystem battleGameSystem;

    // Панели (экранов)
    public GameObject nameInputScreen;
    public GameObject lobbyScreen;
    public GameObject battleScreen;
    public GameObject battleEndScreen;
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public GameObject drawScreen;
    public Button endMatchButton;
    private List<GameObject> screens;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }

        screens = new List<GameObject>
        {
            nameInputScreen,
            lobbyScreen,
            battleScreen,
            victoryScreen,
            defeatScreen,
            drawScreen,
            battleEndScreen
        };

        endMatchButton.onClick.AddListener(() => ShowLobbyScreen());

    }

    private void Start()
    {
        ShowLobbyScreen();
    }

    public void ShowNameInputScreen()
    {
        nameInputScreen.SetActive(true);
    }

    public void ShowLobbyScreen()
    {
        SetActiveScreen(lobbyScreen);
    }

    public void ShowBattleScreen()
    {
        SetActiveScreen(battleScreen);
        battleGameSystem.StartBattle(); // Запускаем бой
    }

    public void ShowResultScreen(RoundResult reuslt)
    {
        battleEndScreen.SetActive(true);
        if (reuslt == RoundResult.PlayerWin)
        {
            victoryScreen.SetActive(true);
        }
        if (reuslt == RoundResult.EnemyWin)
        {
            defeatScreen.SetActive(true);
        }
        else
        {
            drawScreen.SetActive(true);
        }
    }

    public void SetActiveScreen(GameObject activeScreen)
    {
        foreach (var screen in screens)
        {
            screen.SetActive(screen == activeScreen);
        }
    }
}
