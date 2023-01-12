using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using YG;

public class PurchasePanelController : MonoBehaviour
{
    public PurchaseData Data;
    public TMPro.TextMeshProUGUI Title;
    public TMPro.TextMeshProUGUI Cost;
    public TMPro.TextMeshProUGUI Description;
    public GameObject Icon;
    void Start()
    {
        Title.text = Data.Title;
        Cost.text = Data.PaymentAmount.ToString();
        Description.text = Data.Description;
        if (Data.ImageURI != null && Data.ImageURI != "") 
            Icon.GetComponent<ImageLoadYG>().Load(Data.ImageURI);
    }

    public void Buy()
    {
        Data.Process();
    }
}
