using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleChanger : MonoBehaviour
{
    private static LocaleChanger _instance;
    public static LocaleChanger Instance
    {
        get
        {
            if (_instance == null)
            {
                //Debug.Log("Sussy");
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            //throw new System.Exception("SaveDataManager already exists");
        }
    }

    public void ChangeLocale(int id)
    {
        SetLocale(id);
        StartCoroutine(SetLocale(id));
    }

    private static IEnumerator SetLocale(int id)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        PlayerPrefs.SetInt("LocaleID", id);
        PlayerPrefs.Save();
    }
}
