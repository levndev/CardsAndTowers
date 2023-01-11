using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using YG;

public class PurchasePanelController : MonoBehaviour
{
    public string PurchaseID;
    public TMPro.TextMeshProUGUI Title;
    public TMPro.TextMeshProUGUI Cost;
    public TMPro.TextMeshProUGUI Description;
    public GameObject Icon;
    void Start()
    {
        var purchase = YandexGame.PurchaseByID(PurchaseID);
        Title.text = purchase.title;
        Cost.text = purchase.priceValue;
        Description.text = purchase.description;
        Icon.GetComponent<ImageLoadYG>().Load(purchase.imageURI);
    }

    public void Buy()
    {
        if (PurchaseID != null)
            YandexGame.BuyPayments(PurchaseID);
    }
}
