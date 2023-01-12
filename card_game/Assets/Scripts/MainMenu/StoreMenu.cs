using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class StoreMenu : MonoBehaviour
{
    public TMPro.TextMeshProUGUI GoldAmountText;
    void Start()
    {
        SaveDataManager.Instance.GoldChanged += UpdatePlayerStashText;
    }

    private void OnEnable()
    {
        UpdatePlayerStashText();
    }

    private void UpdatePlayerStashText()
    {
        GoldAmountText.text = $"Gold : {SaveDataManager.Instance.GoldAmount}";
    }
}
