using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public MenuScreen MainScreen;
    public MenuScreen CollectionScreen;
    public MenuScreen CurrentScreen;
    public Button BackButton;
    private List<MenuScreen> visitedScreens = new List<MenuScreen>();

    void Start()
    {
        MainScreen.menuManager = this;
        CollectionScreen.menuManager = this;
        BackButton.onClick.AddListener(onBackButtonClick);
        ChangeScreens(MainScreen);
    }

    void Update()
    {

    }

    public void ChangeScreens(MenuScreen nextScreen)
    {
        if(CurrentScreen != null)
            CurrentScreen.Leave();
        nextScreen.Enter();

        if (visitedScreens.Count > 0 && nextScreen == visitedScreens[visitedScreens.Count - 1])
        {
            visitedScreens.RemoveAt(visitedScreens.Count - 1);
        }
        else
        {
            visitedScreens.Add(CurrentScreen);
        }
        CurrentScreen = nextScreen;
    }

    public void onBackButtonClick()
    {
        if(visitedScreens.Count > 0)
            ChangeScreens(visitedScreens[visitedScreens.Count - 1]);
    }
}
