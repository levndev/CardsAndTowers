using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using YG;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "CardsAndTowers/Purchase Data")]
public class PurchaseData : ScriptableObject
{
    [Serializable]
    public class PurchaseResult
    {
        public enum Resource
        {
            Gold,
            Pack,
            Card
        }
        public Resource ResourceType;
        public uint Amount;
        public string ResultId;
    }
    public string Id;
    public string Title;
    public LocalizedString Description;
    public string ImageURI;
    public enum Resource
    {
        Gold,
        Yan,
    }
    public Resource PaymentType;
    public uint PaymentAmount;
    public List<PurchaseResult> Results;

    public void Process()
    {
        switch(PaymentType)
        {
            case Resource.Gold:
                var saveData = SaveDataManager.Instance;
                if (saveData.GoldAmount >= PaymentAmount)
                {
                    saveData.GoldAmount -= PaymentAmount;
                    saveData.UpdateGold();
                    YandexGame.PurchaseSuccessEvent?.Invoke(Id);
                }
                break;
            case Resource.Yan:
                YandexGame.BuyPayments(Id);
                break;
        }
    }
}
