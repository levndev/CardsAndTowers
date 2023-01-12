using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using YG;
using static PurchaseData;

public class PurchaseManager : MonoBehaviour
{
    public Action<string, uint> PackBought;
    private static PurchaseManager _instance;
    public static PurchaseManager Instance
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
    public Dictionary<string, PurchaseData> Purchases = new ();
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
    void Start()
    {
        Purchases = Resources.LoadAll<PurchaseData>("Store").ToDictionary(i => i.Id);
        YandexGame.PurchaseSuccessEvent += PurchaseSuccess;
    }

    private void PurchaseSuccess(string id)
    {
        if (Purchases.TryGetValue(id, out var purchase))
        {
            var saveData = SaveDataManager.Instance;
            foreach (var result in purchase.Results)
            {
                switch (result.ResourceType)
                {
                    case PurchaseResult.Resource.Gold:
                        saveData.GoldAmount += result.Amount;
                        saveData.UpdateGold();
                        break;
                    case PurchaseResult.Resource.Card:
                        break;
                    case PurchaseResult.Resource.Pack:
                        PackBought?.Invoke(result.ResultId, result.Amount);
                        break;
                }
            }
        }
        else
        {
            throw new System.Exception("Epic fail");
        }
    }
}
