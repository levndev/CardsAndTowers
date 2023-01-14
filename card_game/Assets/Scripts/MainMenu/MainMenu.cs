using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SettingsMenu SettingsMenu;
    void Start()
    {
        Time.timeScale = 1;
        SettingsMenu.LoadData();
    }
}
