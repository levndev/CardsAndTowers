using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class StoreTabController : MonoBehaviour
{
    public GameObject Content;
    public GameObject PurchasePanel;
    public List<PurchaseData> Purchases = new();
    private void Populate()
    {
        Clear();
        foreach(var data in Purchases)
        {
            if (PurchaseManager.Instance.Purchases.ContainsKey(data.Id))
            {
                var panel = Instantiate(PurchasePanel, Content.transform);
                panel.GetComponent<PurchasePanelController>().Data = data;
            }
        }
    }

    private void Clear()
    {
        for (var i = 0; i < Content.transform.childCount; i++)
        {
            var child = Content.transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    private void OnEnable()
    {
        YandexGame.GetPaymentsEvent += Populate;
        if (YandexGame.SDKEnabled)
        {
            Populate();
        }
    }

    private void OnDisable() => YandexGame.GetPaymentsEvent -= Populate;
}
