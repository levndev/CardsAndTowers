using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YG;
public class PurchaseManager : MonoBehaviour
{
    public TextAsset PurchaseData;
    private Dictionary<string, PurchaseResult> Purchases = new ();
    void Start()
    {

        var purchaseList = JsonUtility.FromJson<PurchaseResultList>(PurchaseData.text);
        foreach (var purchase in purchaseList.Data)
        {
            Purchases.Add(purchase.Id, purchase);
        }
        YandexGame.PurchaseSuccessEvent += PurchaseSuccess;
    }

    private void PurchaseSuccess(string id)
    {
        if (Purchases.TryGetValue(id, out var purchase))
        {
            switch(purchase.Resource)
            {
                case PurchaseResult.ResourceType.Gold:
                    SaveDataManager.Instance.GoldAmount += purchase.Amount;
                    SaveDataManager.Instance.UpdateGold();
                    break;
                case PurchaseResult.ResourceType.Pack:
                    break;
                case PurchaseResult.ResourceType.Card:
                    break;
            }
        }
        else
        {
            throw new System.Exception("Epic fail");
        }
    }
}
