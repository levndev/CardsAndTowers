using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class StoreTabController : MonoBehaviour
{
    public GameObject Content;
    public GameObject PurchasePanel;

    private void Populate()
    {
        Clear();
        for (var i = 0; i < YandexGame.PaymentsData.id.Length; i++)
        {
            var id = YandexGame.PaymentsData.id[i];
            var panel = Instantiate(PurchasePanel, Content.transform);
            panel.GetComponent<PurchasePanelController>().PurchaseID = id;
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
