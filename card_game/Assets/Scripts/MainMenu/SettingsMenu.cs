using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Editor;
using UnityEngine.Localization.Settings;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer Mixer;
    public Slider MasterSlider;
    public Slider SFXSlider;
    public Slider MusicSlider;
    public TMP_Dropdown LanguageDropdown;
    public LocaleChanger localeChanger;


    public void MasterVolumeChanged()
    {
        Mixer.SetFloat("MasterVolume", Mathf.Log10(MasterSlider.value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", MasterSlider.value);
    }
    public void SFXVolumeChanged()
    {
        Mixer.SetFloat("SFXVolume", Mathf.Log10(SFXSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
    }
    public void MusicVolumeChanged()
    {
        Mixer.SetFloat("MusicVolume", Mathf.Log10(MusicSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            Mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
        if (PlayerPrefs.HasKey("SFXVolume"))
            Mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
        if (PlayerPrefs.HasKey("MusicVolume"))
            Mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        if (PlayerPrefs.HasKey("LocaleID"))
        {
            LanguageDropdown.value = PlayerPrefs.GetInt("LocaleID");
            localeChanger.ChangeLocale(PlayerPrefs.GetInt("LocaleID"));
        }
    }
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (PlayerPrefs.HasKey("SFXVolume"))
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        if (PlayerPrefs.HasKey("MusicVolume"))
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("LocaleID"))
        {
            LanguageDropdown.value = PlayerPrefs.GetInt("LocaleID");
            localeChanger.ChangeLocale(PlayerPrefs.GetInt("LocaleID"));
        }
    }

   

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    //public void ChangeLocale(int id)
    //{
    //    SetLocale(id);
    //    StartCoroutine(SetLocale(id));
    //}

    public void OnLocaleDropdownChange(int id)
    {
        localeChanger.ChangeLocale(id);
    }

    //private static IEnumerator SetLocale(int id)
    //{
    //    yield return LocalizationSettings.InitializationOperation;
    //    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    //    PlayerPrefs.SetInt("LocaleID", id);
    //    PlayerPrefs.Save();

    //}
}
