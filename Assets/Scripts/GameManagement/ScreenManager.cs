// ScreenManager.cs
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    public GameObject splashScreen;
    public GameObject loadingScreen;
    public GameObject registrationScreen;
    public GameObject nameChangeScreen;
    public GameObject lobbyScreen;
    public GameObject battleScreen;
    public GameObject queueScreen;
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public GameObject drawScreen;
    public GameObject errorScreen;

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
    }

    private void Start()
    {
        HideAllScreens();
    }

    public void ShowScreen(GameObject screen)
    {
        HideAllScreens();
        screen.SetActive(true);
    }

    private void HideAllScreens()
    {
        splashScreen.SetActive(false);
        loadingScreen.SetActive(false);
        registrationScreen.SetActive(false);
        nameChangeScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        battleScreen.SetActive(false);
        queueScreen.SetActive(false);
        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);
        drawScreen.SetActive(false);
        errorScreen.SetActive(false);
    }
}