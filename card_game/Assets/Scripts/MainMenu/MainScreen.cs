using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : MenuScreen
{
    public Button ExitButton;
    public Button CollectionButton;
    public Button PlayButton;
    

    void Start()
    {
        PlayButton.onClick.AddListener(OnPlayButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
        CollectionButton.onClick.AddListener(OnCollectionButtonClick);
    }

    void Update()
    {
        
    }

    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnCollectionButtonClick()
    {
        menuManager.ChangeScreens(menuManager.CollectionScreen);
    }

    public override void onScreenEnter()
    {
        
    }
}