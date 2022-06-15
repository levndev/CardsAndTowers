using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public delegate void LevelButtonClickHandler(int scene);
    public LevelButtonClickHandler ClickHandler;
    public int scene;
    public void ButtonClick()
    {
        ClickHandler?.Invoke(scene);
    }
}
