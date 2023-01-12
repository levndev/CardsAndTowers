using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using YG;

public class StoreMenu : MonoBehaviour
{
    public TMPro.TextMeshProUGUI GoldAmountText;
    [SerializeField] private LocalizedString localGoldAmount;

    void Start()
    {
        SaveDataManager.Instance.GoldChanged += UpdatePlayerStashText;
    }

    private void OnEnable()
    {
        localGoldAmount.StringChanged += UpdateLocalGoldAmount;
        localGoldAmount.Arguments = new object[1] { SaveDataManager.Instance.GoldAmount };
        UpdatePlayerStashText();
    }

    private void OnDisable()
    {
        localGoldAmount.StringChanged -= UpdateLocalGoldAmount;
    }

    private void UpdatePlayerStashText()
    {
        localGoldAmount.Arguments[0] = SaveDataManager.Instance.GoldAmount;
        localGoldAmount.RefreshString();
        //GoldAmountText.text = $"Gold : {SaveDataManager.Instance.GoldAmount}";
    }

    private void UpdateLocalGoldAmount(string s)
    {
        GoldAmountText.text = s;
    }
}
