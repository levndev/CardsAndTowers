using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScreen : MenuScreen
{
    public LevelButton[] Levels;

    void Start()
    {
        foreach(var level in Levels)
        {
            level.ClickHandler += LevelButtonClick;
        }
    }

    public void LevelButtonClick(int scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
