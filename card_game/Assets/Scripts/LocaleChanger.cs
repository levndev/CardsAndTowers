using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleChanger : MonoBehaviour
{
    // Start is called before the first frame update
    

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
