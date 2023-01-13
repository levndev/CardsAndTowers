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
        var op = Data.Description.GetLocalizedStringAsync();
        if (op.IsDone)
            Description.text = op.Result;
        else
            op.Completed += (op) => Description.text = op.Result;
        if (Data.ImageURI != null && Data.ImageURI != "") 
            Icon.GetComponent<ImageLoadYG>().Load(Data.ImageURI);
    }

    public void Buy()
    {
        Data.Process();
    }
}
